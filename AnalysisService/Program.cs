using AnalysisService.Application.Consumers;
using AnalysisService.Application.Interfaces;
using AnalysisService.Infrastructure.AI;
using AnalysisService.Infrastructure.Data;
using AnalysisService.Infrastructure.Repositories;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AnalysisDatabase")));

builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<FeedbackSubmittedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var rabbitMqUsername = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var rabbitMqPassword = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(
            rabbitMqHost,
            "/",
            h =>
            {
                h.Username(rabbitMqUsername);
                h.Password(rabbitMqPassword);
            });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<AzureOpenAiOptions>(
    builder.Configuration.GetSection("AzureOpenAI"));

builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IFeedbackAnalysisSearchService, FeedbackAnalysisSearchService>();

var app = builder.Build();

var azureOpenAiEndpoint = builder.Configuration["AzureOpenAI:Endpoint"];
var azureOpenAiDeployment = builder.Configuration["AzureOpenAI:DeploymentName"];

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    db.Database.Migrate();
}

await EnsureFeedbackAnalysisIndexAsync(app.Services, app.Logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task EnsureFeedbackAnalysisIndexAsync(IServiceProvider services, ILogger logger)
{
    const string indexName = "feedback-analysis-index";
    const string vectorFieldName = "embedding";
    const string vectorProfileName = "feedback-vector-profile";
    const string vectorAlgorithmName = "feedback-hnsw";
    const int embeddingDimensions = 1536;

    using var scope = services.CreateScope();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<AzureOpenAiOptions>>().Value;

    if (string.IsNullOrWhiteSpace(options.AiSearchEndpoint) || string.IsNullOrWhiteSpace(options.AiSearchApiKey))
    {
        logger.LogWarning("AI Search configuration is incomplete. AiSearchEndpoint or AiSearchApiKey is missing.");
        return;
    }

    var client = new SearchIndexClient(new Uri(options.AiSearchEndpoint), new AzureKeyCredential(options.AiSearchApiKey));

    try
    {
        var existingIndex = await client.GetIndexAsync(indexName);

        var hasEmbeddingField = existingIndex.Value.Fields.Any(field => field.Name == vectorFieldName);
        if (!hasEmbeddingField)
        {
            existingIndex.Value.Fields.Add(new SearchField(vectorFieldName, SearchFieldDataType.Collection(SearchFieldDataType.Single))
            {
                IsSearchable = true,
                VectorSearchDimensions = embeddingDimensions,
                VectorSearchProfileName = vectorProfileName
            });

            existingIndex.Value.VectorSearch ??= new VectorSearch();

            if (!existingIndex.Value.VectorSearch.Algorithms.Any(a => a.Name == vectorAlgorithmName))
            {
                existingIndex.Value.VectorSearch.Algorithms.Add(new HnswAlgorithmConfiguration(vectorAlgorithmName));
            }

            if (!existingIndex.Value.VectorSearch.Profiles.Any(p => p.Name == vectorProfileName))
            {
                existingIndex.Value.VectorSearch.Profiles.Add(new VectorSearchProfile(vectorProfileName, vectorAlgorithmName));
            }

            await client.CreateOrUpdateIndexAsync(existingIndex.Value);
            logger.LogInformation("AI Search index '{IndexName}' updated with vector field '{VectorFieldName}'.", indexName, vectorFieldName);
            return;
        }

        logger.LogInformation("AI Search index '{IndexName}' already exists with vector field '{VectorFieldName}'.", indexName, vectorFieldName);
        return;
    }
    catch (RequestFailedException ex) when (ex.Status == 404)
    {
    }

    var fields = new List<SearchField>
    {
        new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
        new SimpleField("feedbackId", SearchFieldDataType.String) { IsFilterable = true },
        new SearchableField("sentiment") { IsFilterable = true, IsSortable = true },
        new SearchableField("summary"),
        new SimpleField("confidence", SearchFieldDataType.Double) { IsFilterable = true, IsSortable = true },
        new SearchableField("primaryTopic") { IsFilterable = true },
        new SearchableField("reason"),
        new SimpleField("createdAtUtc", SearchFieldDataType.DateTimeOffset) { IsFilterable = true, IsSortable = true },
        new SimpleField("priority", SearchFieldDataType.String) { IsFilterable = true, IsSortable = true },
        new SearchField(vectorFieldName, SearchFieldDataType.Collection(SearchFieldDataType.Single))
        {
            IsSearchable = true,
            VectorSearchDimensions = embeddingDimensions,
            VectorSearchProfileName = vectorProfileName
        }
    };

    var index = new SearchIndex(indexName, fields)
    {
        VectorSearch = new VectorSearch
        {
            Algorithms =
            {
                new HnswAlgorithmConfiguration(vectorAlgorithmName)
            },
            Profiles =
            {
                new VectorSearchProfile(vectorProfileName, vectorAlgorithmName)
            }
        }
    };

    await client.CreateIndexAsync(index);
    logger.LogInformation("AI Search index '{IndexName}' created with vector field '{VectorFieldName}'.", indexName, vectorFieldName);
}
