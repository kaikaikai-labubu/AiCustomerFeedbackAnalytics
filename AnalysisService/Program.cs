using Microsoft.EntityFrameworkCore;
using AnalysisService.Infrastructure.Data;
using AnalysisService.Application.Interfaces;
using AnalysisService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AnalysisDatabase")));

builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
