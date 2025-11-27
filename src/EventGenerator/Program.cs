using EventGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<EventSenderService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7216/"); // URL EventProcessor
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHostedService<EventGeneratorService>();

builder.Services.AddControllers();
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
