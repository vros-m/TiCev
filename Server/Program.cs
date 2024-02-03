using TiCev.Server.Settings;
using TiCev.Server.Error;
using TiCev.Server.Business.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TiCev.Server.Business.Repos;

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

/* builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value); */

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    /*  var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
     var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
     clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1); */
    string ConnectionString = "mongodb://localhost:27017";
    return new MongoClient(ConnectionString);
});

builder.Services.AddScoped<VideoRepo, VideoRepo>();
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


