using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ===== Configure middleware pipeline (order matters) =====
// 1. Error handling middleware (first) - catches exceptions from downstream
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Authentication middleware (second) - validates bearer tokens
app.UseMiddleware<AuthenticationMiddleware>();

// 3. Logging middleware (last) - records requests and responses
app.UseMiddleware<LoggingMiddleware>();

var users = new Dictionary<int, User>
{
    { 1, new User { Id = 1, Name = "John Doe", Email = "john.doe@techhive.com", Department = "Engineering", Position = "Senior Developer" } },
    { 2, new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@techhive.com", Department = "HR", Position = "HR Manager" } }
};

var nextUserId = 3;

static string? ValidateUserInput(User? user)
{
    if (user is null)
        return "Data pengguna tidak valid.";

    if (string.IsNullOrWhiteSpace(user.Name))
        return "Nama pengguna tidak boleh kosong.";

    if (user.Name.Trim().Length < 2)
        return "Nama pengguna minimal 2 karakter.";

    if (string.IsNullOrWhiteSpace(user.Email))
        return "Email pengguna tidak boleh kosong.";

    if (!Regex.IsMatch(user.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        return "Format email tidak valid.";

    return null;
}

app.MapGet("/api/users", () =>
{
    try
    {
        var result = users.Values
            .OrderBy(u => u.Id)
            .ToList();

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Terjadi kesalahan saat mengambil data pengguna: {ex.Message}");
    }
})
.WithName("GetAllUsers")
.WithOpenApi()
.WithDescription("Mengambil semua data pengguna");

app.MapGet("/api/users/{id}", (int id) =>
{
    try
    {
        if (id <= 0)
            return Results.BadRequest(new { message = "ID pengguna harus lebih dari 0." });

        if (!users.TryGetValue(id, out var user))
            return Results.NotFound(new { message = $"User dengan ID {id} tidak ditemukan." });

        return Results.Ok(user);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Terjadi kesalahan saat mengambil user dengan ID {id}: {ex.Message}");
    }
})
.WithName("GetUserById")
.WithOpenApi()
.WithDescription("Mengambil satu pengguna berdasarkan ID");

app.MapPost("/api/users", (User newUser) =>
{
    try
    {
        var validationMessage = ValidateUserInput(newUser);
        if (validationMessage is not null)
            return Results.BadRequest(new { message = validationMessage });

        var normalizedEmail = newUser.Email.Trim();
        if (users.Values.Any(u => string.Equals(u.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)))
            return Results.Conflict(new { message = $"Email '{normalizedEmail}' sudah terdaftar." });

        newUser.Id = nextUserId++;
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.UpdatedAt = newUser.CreatedAt;
        users[newUser.Id] = newUser;

        return Results.Created($"/api/users/{newUser.Id}", newUser);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Terjadi kesalahan saat menambah pengguna: {ex.Message}");
    }
})
.WithName("CreateUser")
.WithOpenApi()
.WithDescription("Menambahkan pengguna baru");

app.MapPut("/api/users/{id}", (int id, User updatedUser) =>
{
    try
    {
        if (id <= 0)
            return Results.BadRequest(new { message = "ID pengguna harus lebih dari 0." });

        var validationMessage = ValidateUserInput(updatedUser);
        if (validationMessage is not null)
            return Results.BadRequest(new { message = validationMessage });

        if (!users.TryGetValue(id, out var existingUser))
            return Results.NotFound(new { message = $"User dengan ID {id} tidak ditemukan." });

        var normalizedEmail = updatedUser.Email.Trim();
        if (users.Values.Any(u => u.Id != id && string.Equals(u.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)))
            return Results.Conflict(new { message = $"Email '{normalizedEmail}' sudah dipakai user lain." });

        existingUser.Name = updatedUser.Name.Trim();
        existingUser.Email = normalizedEmail;
        existingUser.Department = updatedUser.Department;
        existingUser.Position = updatedUser.Position;
        existingUser.UpdatedAt = DateTime.UtcNow;

        return Results.Ok(existingUser);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Terjadi kesalahan saat memperbarui user dengan ID {id}: {ex.Message}");
    }
})
.WithName("UpdateUser")
.WithOpenApi()
.WithDescription("Memperbarui data pengguna yang sudah ada");

app.MapDelete("/api/users/{id}", (int id) =>
{
    try
    {
        if (id <= 0)
            return Results.BadRequest(new { message = "ID pengguna harus lebih dari 0." });

        if (!users.TryGetValue(id, out _))
            return Results.NotFound(new { message = $"User dengan ID {id} tidak ditemukan." });

        users.Remove(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Terjadi kesalahan saat menghapus user dengan ID {id}: {ex.Message}");
    }
})
.WithName("DeleteUser")
.WithOpenApi()
.WithDescription("Menghapus pengguna berdasarkan ID");

app.MapGet("/api/health", () =>
{
    try
    {
        return Results.Ok(new { status = "API is healthy", timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Terjadi kesalahan saat mengecek status API: {ex.Message}");
    }
})
.WithName("HealthCheck")
.WithOpenApi()
.WithDescription("Memeriksa status API");

app.Run();

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
