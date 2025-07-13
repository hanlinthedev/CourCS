using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using UserManagement.Utils;
using UserManagement.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline in the correct order
// 1. Error handling middleware (first to catch all exceptions)
app.UseErrorHandling();

// Configure Swagger (before authentication to allow access)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. Authentication middleware (after error handling, before business logic)
app.UseCustomAuthentication();

// 3. Request logging middleware (last to log all processed requests)
app.UseRequestLogging();

// In-memory storage for users (replace with database in production)
var users = new List<User>();

// GET all users
app.MapGet("/users", () => users)
    .WithName("GetAllUsers")
    .WithOpenApi();

// GET user by ID
app.MapGet("/users/{id}", (Guid id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is null ? Results.NotFound() : Results.Ok(user);
})
.WithName("GetUserById")
.WithOpenApi();

// POST new user
app.MapPost("/users", (User user) =>
{
    var validationResult = Utils.ValidateUser(user);
    if (!string.IsNullOrEmpty(validationResult))
        return Results.BadRequest(new { error = validationResult });

    user.Id = Guid.NewGuid();
    users.Add(user);
    return Results.Created($"/users/{user.Id}", user);
})
.WithName("CreateUser")
.WithOpenApi();

// PUT update user
app.MapPut("/users/{id}", (Guid id, User updatedUser) =>
{
    var existingUser = users.FirstOrDefault(u => u.Id == id);
    if (existingUser is null) return Results.NotFound();

    var validationResult = Utils.ValidateUser(updatedUser);
    if (!string.IsNullOrEmpty(validationResult))
        return Results.BadRequest(new { error = validationResult });

    existingUser.Name = updatedUser.Name;
    existingUser.Email = updatedUser.Email;
    existingUser.Age = updatedUser.Age;

    return Results.Ok(existingUser);
})
.WithName("UpdateUser")
.WithOpenApi();

// DELETE user
app.MapDelete("/users/{id}", (Guid id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    users.Remove(user);
    return Results.Ok();
})
.WithName("DeleteUser")
.WithOpenApi();

// Test endpoint to trigger an exception for testing error handling
app.MapGet("/test/error", () =>
{
    throw new InvalidOperationException("This is a test exception for middleware testing.");
})
.WithName("TestError")
.WithOpenApi();

app.Run();

// User model
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
}
