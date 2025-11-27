using EventProcessor.Infrastructure.Data;
using EventProcessor.Infrastructure.Repositories;
using EventProcessor.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<IIncidentRepository, IncidentRepository>();

builder.Services.AddSingleton<EventProcessorService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<EventProcessorService>());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Дополнительная защита от бесконечных цепочек связанных моделей в json
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
