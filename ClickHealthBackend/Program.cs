using ClickHealthBackend.Data;
using ClickHealth.Server.Models;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Implementations;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Implementations;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using OtpNet;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7236") // your Blazor port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. Controllers & Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ClickHealth API", Version = "v1" });
});

// --- 3. Configuration & Dependency Injection ---

// MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<MongoDbContext>();

// SMTP / Email service
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Services & Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContentService, ContentService>();

var app = builder.Build();

// --- 4. Seed Default Admin ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var usersCollection = context.Users;

    var existingAdmin = await usersCollection
        .Find(u => u.Email == "sunilofficial781@gmail.com" && u.Role == UserRole.Admin)
        .FirstOrDefaultAsync();

    if (existingAdmin == null)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin@123");

        var adminUser = new User
        {
            Email = "sunilofficial781@gmail.com",
            Name = "Admin",
            Phone = "9008284717",
            Role = UserRole.Admin,
            Specialty = "Admin",
            Territory = "Global",
            IsActive = true,
            IsApproved = true,
            Status = UserStatus.Approved,
            PreferredLanguage = "English",
            CreatedAt = DateTime.UtcNow,
            Password = hashedPassword,
            MustResetPassword = false
        };

        await usersCollection.InsertOneAsync(adminUser);
        Console.WriteLine("✅ Default Admin created: sunilofficial781@gmail.com / admin@123");
    }
}

// --- 5. Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClickHealth API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// --- 6. Auto-create collections with schema validation ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var db = context.Database;

    void CreateCollectionIfNotExists<T>(IMongoDatabase database, string collectionName)
    {
        var existing = database.ListCollectionNames().ToList();
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

                database.RunCommand(command);
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

    CreateCollectionIfNotExists<User>(db, "Users");
    CreateCollectionIfNotExists<AuditLog>(db, "AuditLog");

    // Create collections without schema
    db.CreateCollection("Campaigns");
    db.CreateCollection("Contents");
}

app.Run();
