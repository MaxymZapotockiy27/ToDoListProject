using System.ComponentModel.DataAnnotations;

namespace ToDoListProj.Models
{
    public class Project
    {

        public int Id { get; set; }
        [Required]

        public string Name { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<TaskItem>? Tasks { get; set; }

    }
}
