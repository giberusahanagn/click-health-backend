using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

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

    CreateCollectionIfNotExists<User>(db, "Users");
    CreateCollectionIfNotExists<Campaign>(db, "Campaigns");
    CreateCollectionIfNotExists<Content>(db, "Contents");
}

app.Run();

void CreateCollectionIfNotExists<T>(IMongoDatabase db, string collectionName)
{
    var existing = db.ListCollectionNames().ToList();
    if (!existing.Contains(collectionName))
    {
        var schema = MongoSchemaGenerator.GenerateSchema<T>();

        var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
        {
            { "create", collectionName },
            { "validator", new BsonDocument { { "$jsonSchema", schema } } }
        });

        db.RunCommand(command);
    }
}
