using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<HagglehaulDatabaseSettings>(
    builder.Configuration.GetSection("HagglehaulDatabase"));

// Create singleton IMongoDatabase from HagglehaulDatabaseSettings
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<HagglehaulDatabaseSettings>>();
    var client = new MongoClient(settings.Value.ConnectionString);
    return client.GetDatabase(settings.Value.DatabaseName);
});

builder.Services.AddSingleton<MongoTestService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
