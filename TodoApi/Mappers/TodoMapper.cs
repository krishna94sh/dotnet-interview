using TodoApi.Models;
using TodoApi.Models.DTOs;

namespace TodoApi.Mappers
{
    public static class TodoMapper
    {
        public static TodoResponse ToResponse(Todo todo)
        {
            return new TodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            };
        }

        public static List<TodoResponse> ToResponseList(List<Todo> todos)
        {
            return todos.Select(ToResponse).ToList();
        }
    }
}
