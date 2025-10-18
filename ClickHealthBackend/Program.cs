<<<<<<< Updated upstream
﻿using ClickHealthBackend.Data;
using ClickHealth.Server.Models;

﻿using ClickHealthBackend.Data;

﻿using ClickHealth.Server.Models;
=======
>>>>>>> Stashed changes
using ClickHealthBackend.Data;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Implementations;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Implementations;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
<<<<<<< Updated upstream
using MongoDB.Bson;

=======
>>>>>>> Stashed changes
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< Updated upstream

// --- 1. Configuration & Dependency Injection ---

// Configure MongoDbSettings
=======
// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

>>>>>>> Stashed changes
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
<<<<<<< Updated upstream

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
=======
>>>>>>> Stashed changes
builder.Services.AddScoped<IContentService, ContentService>();

var app = builder.Build();

<<<<<<< Updated upstream
// --- 4. Seed Default Admin ---
using (var scope = app.Services.CreateScope())
=======
// Middleware
if (app.Environment.IsDevelopment())
>>>>>>> Stashed changes
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClickHealth API v1"); c.RoutePrefix = "swagger"; });
}

<<<<<<< Updated upstream
    var existingAdmin = await usersCollection
        .Find(u => u.Email == "sunilofficial781@gmail.com" && u.Role == UserRole.Admin)
        .FirstOrDefaultAsync();

    if (existingAdmin == null)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin@123");

=======
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed admin & create collections
await SeedAdminUserAsync(app);
CreateCollectionsIfNotExists(app);

app.Run();

// ---------------------------
// Helper Methods
// ---------------------------

static async Task SeedAdminUserAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var users = context.Users;

    var existingAdmin = await users.Find(u => u.Email == "admin@clickhealth.com" && u.Role == UserRole.Admin)
                                  .FirstOrDefaultAsync();

    if (existingAdmin == null)
    {
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            Password = hashedPassword,
            MustResetPassword = false
        };

        await usersCollection.InsertOneAsync(adminUser);
        Console.WriteLine("✅ Default Admin created: sunilofficial781@gmail.com / admin@123");
    }
}

// --- 5. Middleware ---
if (app.Environment.IsDevelopment())
=======
            Password = BCrypt.Net.BCrypt.HashPassword("admin@123")
        };

        await users.InsertOneAsync(adminUser);
        Console.WriteLine("✅ Default Admin created");
    }
}

static void CreateCollectionsIfNotExists(WebApplication app)
>>>>>>> Stashed changes
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>().Database;

    foreach (var name in new[] { "Users", "Campaigns", "Contents", "AuditLog" })
    {
<<<<<<< Updated upstream
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
=======
        if (!db.ListCollectionNames().ToList().Contains(name))
        {
            db.CreateCollection(name);
            Console.WriteLine($"✅ Created collection: {name}");
        }
        else
        {
            Console.WriteLine($"ℹ️ Collection {name} already exists.");
        }
    }
}
>>>>>>> Stashed changes
