using TiCev.Server.Error;
using TiCev.Server.Business.Services;
using MongoDB.Driver;
using TiCev.Server.Business.Repos;
using MongoDB.Bson;
using TiCev.Server.Data.Entities;
using System.Net.WebSockets;


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



builder.Services.AddSingleton<IMongoClient>(sp =>
{

    string ConnectionString = "mongodb://localhost:27017";
    return new MongoClient(ConnectionString);
});

builder.Services.AddSingleton<SocketService, SocketService>();

builder.Services.AddScoped<VideoRepo, VideoRepo>();
builder.Services.AddScoped<VideoService, VideoService>();
builder.Services.AddScoped<UserRepo,UserRepo>();
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<TagRepo, TagRepo>();
builder.Services.AddScoped<TagService, TagService>();

var app = builder.Build();

var mongoDatabase = app.Services.GetService<IMongoClient>()!.GetDatabase(Constants.ConstObj.DatabaseName);
var usersCollection = mongoDatabase.GetCollection<BsonDocument>("users");
var usernameIndexModel = new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Text("Username"),
            new CreateIndexOptions { Unique = true }
        );
usersCollection.Indexes.CreateOne(usernameIndexModel);


var videosCollection = mongoDatabase.GetCollection<Video>("videos");
videosCollection.Indexes.CreateMany([new CreateIndexModel<Video>(
    Builders<Video>.IndexKeys.Text(v => v.Title)),
new CreateIndexModel<Video>(Builders<Video>.IndexKeys.Ascending(v=>v.Tags))
]);

var tagCollection = mongoDatabase.GetCollection<TiCev.Server.Data.Entities.Tag>("tags");
tagCollection.Indexes.CreateMany([new CreateIndexModel<TiCev.Server.Data.Entities.Tag>(
    Builders<TiCev.Server.Data.Entities.Tag>.IndexKeys.Text(t=>t.TagName)
)]);

app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path=="/SocketService") // Or whatever path you want to handle WebSocket requests
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocketService = context.RequestServices.GetRequiredService<SocketService>();
            await webSocketService.HandleConnection(context);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next();
    }
});
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


