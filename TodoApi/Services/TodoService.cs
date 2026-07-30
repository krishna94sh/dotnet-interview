using Microsoft.Data.Sqlite;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly string _connectionString = "Data Source=todos.db";

        public Todo CreateTodo(Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                VALUES (@title, @description, @completed, @createdAt);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("@title", todo.Title);
            command.Parameters.AddWithValue("@description", todo.Description ?? string.Empty);
            command.Parameters.AddWithValue("@completed", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));

            var id = Convert.ToInt32(command.ExecuteScalar());

            todo.Id = id;
            todo.CreatedAt = DateTime.UtcNow;

            return todo;
        }

        public List<Todo> GetAllTodos()
        {
            var todos = new List<Todo>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                todos.Add(new Todo
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    IsCompleted = reader.GetInt32(3) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }

            return todos;
        }

        public Todo? GetTodoById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Todo
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                IsCompleted = reader.GetInt32(3) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(4))
            };
        }

        public Todo UpdateTodo(int id, Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Todos
                SET Title = @title,
                    Description = @description,
                    IsCompleted = @completed
                WHERE Id = @id;
            ";

            command.Parameters.AddWithValue("@title", todo.Title);
            command.Parameters.AddWithValue("@description", todo.Description ?? string.Empty);
            command.Parameters.AddWithValue("@completed", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();

            todo.Id = id;

            return todo;
        }

        public bool DeleteTodo(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
