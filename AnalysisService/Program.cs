using AnalysisService.Application.Interfaces;
using AnalysisService.Application.Consumers;
using AnalysisService.Infrastructure.Data;
using AnalysisService.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AnalysisDatabase")));

builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
