using System.ComponentModel.DataAnnotations;

namespace ToDoListProj.Models
{
    public class GroupTask
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? GroupImageUrl { get; set; }
        public ICollection<TaskItem>? Tasks { get; set; }
        public ICollection<GroupMember>? Members { get; set; }
    }
}
