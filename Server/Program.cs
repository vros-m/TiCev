using System.Security.Cryptography.X509Certificates;
using Error;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(action =>
{
    action.AddPolicy("CORS", policy =>
    {
        policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

const string connectionUri = "mongodb+srv://admin:admin@cluster0.0ebyeqm.mongodb.net/?retryWrites=true&w=majority";
var settings = MongoClientSettings.FromConnectionString(connectionUri);
// Set the ServerApi field of the settings object to set the version of the Stable API on the client
settings.ServerApi = new ServerApi(ServerApiVersion.V1);
// Create a new client and connect to the server
var client = new MongoClient(settings);
// Send a ping to confirm a successful connection
try {
  var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
  Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
} catch (Exception ex) {
  Console.WriteLine(ex);
}

app.UseMiddleware<DefaultErrorHandler>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CORS");
app.UseHttpsRedirection();
app.Run();


