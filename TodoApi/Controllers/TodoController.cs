using Microsoft.AspNetCore.Mvc;
using TodoApi.Mappers;
using TodoApi.Models;
using TodoApi.Models.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }


        // GET: api/todos
        [HttpGet]
        public IActionResult GetAllTodos()
        {
            var todos = _todoService.GetAllTodos();

            var response = TodoMapper.ToResponseList(todos);

            return Ok(response);
        }


        // GET: api/todos/{id}
        [HttpGet("{id}")]
        public IActionResult GetTodoById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid todo id.");
            }

            var todo = _todoService.GetTodoById(id);

            if (todo == null)
            {
                return NotFound();
            }

            return Ok(TodoMapper.ToResponse(todo));
        }


        // POST: api/todos
        [HttpPost]
        public IActionResult CreateTodo([FromBody] CreateTodoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted
            };


            var createdTodo = _todoService.CreateTodo(todo);


            return CreatedAtAction(
                nameof(GetTodoById),
                new { id = createdTodo.Id },
                TodoMapper.ToResponse(createdTodo)
            );
        }


        // PUT: api/todos/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTodo(
            int id,
            [FromBody] UpdateTodoRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid todo id.");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var existingTodo = _todoService.GetTodoById(id);

            if (existingTodo == null)
            {
                return NotFound();
            }


            var todo = new Todo
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted
            };


            var updatedTodo = _todoService.UpdateTodo(id, todo);


            return Ok(TodoMapper.ToResponse(updatedTodo));
        }


        // DELETE: api/todos/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTodo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid todo id.");
            }


            var deleted = _todoService.DeleteTodo(id);

            if (!deleted)
            {
                return NotFound();
            }


            return Ok(new
            {
                message = "Todo deleted successfully."
            });
        }
    }
}
