using EventProcessor.Infrastructure.Data;
using EventProcessor.Infrastructure.Repositories;
using EventProcessor.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString),
    contextLifetime: ServiceLifetime.Singleton); // ← ВАЖНО!

builder.Services.AddSingleton<IIncidentRepository, IncidentRepository>(); // ← Singleton!

// 2. EventProcessorService как Singleton
builder.Services.AddSingleton<EventProcessorService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<EventProcessorService>());


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
