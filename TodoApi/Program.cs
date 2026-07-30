using Microsoft.Data.Sqlite;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Dependency Injection
builder.Services.AddScoped<ITodoService, TodoService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize SQLite Database
InitializeDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void InitializeDatabase()
{
    var connectionString = "Data Source=todos.db";

    using var connection = new SqliteConnection(connectionString);
    connection.Open();

    var command = connection.CreateCommand();

    command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Todos
        (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            Description TEXT,
            IsCompleted INTEGER NOT NULL DEFAULT 0,
            CreatedAt TEXT NOT NULL
        );
    ";

    command.ExecuteNonQuery();

    Console.WriteLine("Database initialized successfully.");
}
