
﻿using ClickHealthBackend.Data;

﻿using ClickHealth.Server.Models;
using ClickHealthBackend.Data;
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


// --- 1. Configuration & Dependency Injection ---

// Configure MongoDbSettings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Configure SmtpSettings (Needed for EmailService)
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// FIX: Explicitly register IMongoClient as a Singleton.
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Register MongoDbContext as singleton
builder.Services.AddSingleton<MongoDbContext>();

// Register repository and service using Scoped lifetime
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// FIX: Register the Email Service implementation to resolve the dependency in UserService
builder.Services.AddScoped<IEmailService, EmailService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger services

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7236") // your Blazor port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Controllers & Swagger ---
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ClickHealth API", Version = "v1" });
});

// --- DI & Configuration ---
// MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<MongoDbContext>();

// Email
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Services & Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContentService, ContentService>();

var app = builder.Build();

// --- 1. Seed Default Admin ---
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

        // Optional: generate TOTP secret
        // var secretKey = KeyGeneration.GenerateRandomKey(20);
        // var base32Secret = Base32Encoding.ToString(secretKey);

        var adminUser = new User
        {
            Email = "sunilofficial781@gmail.com",
            Name = "Admin",
            Phone = "9008284717",
            Role = UserRole.Admin,
            Specialty = "Admin",
            Territory = "Global",
            IsActive = true,
            IsApproved = true,          // allow login
            Status = UserStatus.Approved, // approved in workflow
            PreferredLanguage = "English",
            CreatedAt = DateTime.UtcNow,
            Password = hashedPassword,
            MustResetPassword = false,
            // TotpSecretKey = base32Secret // optional
        };

        await usersCollection.InsertOneAsync(adminUser);
        Console.WriteLine("✅ Default Admin created: sunilofficial781@gmail.com / admin@123");
    }
}

// --- 2. Middleware ---
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

// --- 3. Auto-create collections with schema validation ---
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
    db.CreateCollection("Campaigns");
    db.CreateCollection("Contents");
}


app.Run();
