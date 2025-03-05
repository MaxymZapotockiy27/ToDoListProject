using System.ComponentModel.DataAnnotations;

namespace ToDoListProj.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
        public int? GroupTaskId { get; set; }
        public GroupTask? GroupTask { get; set; }
    }
}
