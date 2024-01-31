using TiCev.Server.Settings;
using TiCev.Server.Error;
using TiCev.Server.Repositories;
using TiCev.Server.Services;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(action =>
{
    action.AddPolicy("CORS", policy =>
    {
        policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//mongoDB service injection

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
    clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
    return new MongoClient(clientSettings);
});

//services injections
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(settings.DatabaseName);

    InitializeDatabase(database, settings);

    return database;
});

builder.Services.AddSingleton<VideoRepo, VideoRepo>(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new VideoRepo(database, settings.VideoCollectionName);
});

builder.Services.AddScoped<VideoService, VideoService>();

var app = builder.Build();

app.UseMiddleware<DefaultErrorHandler>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseCors("CORS");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


// Database initialization logic (e.g., creating indexes)
void InitializeDatabase(IMongoDatabase db, MongoDBSettings settings)
{

}

