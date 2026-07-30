using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models.DTOs
{
    public class UpdateTodoRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;


        [MaxLength(1000)]
        public string? Description { get; set; }


        public bool IsCompleted { get; set; }
    }
}
