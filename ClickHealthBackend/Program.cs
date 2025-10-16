using ClickHealth.Server.Models;
using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Implementations;
using MongoDB.Bson;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IContentService, ContentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// --- Auto-create collections with schema validation ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var db = context.Database;
    db.CreateCollection("Campaigns"); // No schema
    db.CreateCollection("Contents"); // No schema

    CreateCollectionIfNotExists<User>(db, "Users");
    //CreateCollectionIfNotExists<Campaign>(db, "Campaigns");
    //CreateCollectionIfNotExists<Content>(db, "Contents");
    CreateCollectionIfNotExists<AuditLog>(db, "AuditLog");
    //CreateCollectionIfNotExists<Content>(db, "Contents");
    //CreateCollectionIfNotExists<Content>(db, "Contents");
    //CreateCollectionIfNotExists<Content>(db, "Contents");
}

app.Run();

void CreateCollectionIfNotExists<T>(IMongoDatabase db, string collectionName)
{
    var existing = db.ListCollectionNames().ToList();
    if (!existing.Contains(collectionName))
    {
        try
        {
            var schema = MongoSchemaGenerator.GenerateSchema<T>();
            Console.WriteLine($"Schema for {collectionName}: {schema?.ToJson()}");

            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
            {
                { "create", collectionName },
                { "validator", new BsonDocument { { "$jsonSchema", schema } } }
            });

            db.RunCommand(command);
            Console.WriteLine($"Created collection: {collectionName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating collection {collectionName}: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine($"Collection {collectionName} already exists.");
    }
}