using ClickHealthBackend.Data;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
<<<<<<< HEAD
using MongoDB.Bson;
=======
using ClickHealthBackend.Repositories.Implementations;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Implementations;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
>>>>>>> feature/sahana
using MongoDB.Driver;
using OtpNet;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
// Add services

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
=======
// --- 1. Configuration & Dependency Injection ---

// Configure MongoDbSettings
>>>>>>> feature/sahana
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ClickHealth API", Version = "v1" });
});


var app = builder.Build();

// --- 2. Initial Data Seeding (Requires MongoDbContext) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var usersCollection = context.Users;

    var existingAdmin = await usersCollection
        .Find(u => u.Email == "admin@clickhealth.com" && u.Role == UserRole.Admin)
        .FirstOrDefaultAsync();

    if (existingAdmin == null)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin@123");

        // Generate a TOTP secret key for the admin user
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(secretKey);

        var adminUser = new User
        {
            Email = "sunilofficial781@gmail.com",
            Phone = "9008284717",
            Role = UserRole.Admin,
            Specialty = "Admin",
            Territory = "Global",
            IsActive = true,
            IsApproved = true,
            PreferredLanguage = "English",
            CreatedAt = DateTime.UtcNow,
            Password = hashedPassword,
            //TotpSecretKey = base32Secret
        };

        await usersCollection.InsertOneAsync(adminUser);
        Console.WriteLine("✅ Default Admin created: sunilofficial781@gmail.com / admin@123 (Hashed)");
    }
}


// --- 3. Configure Middleware ---
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
app.Run();
