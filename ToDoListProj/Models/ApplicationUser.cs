using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ToDoListProj.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string FullName { get; set; }

        public string? AvatarUrl { get; set; }  

        public ICollection<TaskItem>? Tasks { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<GroupMember>? GroupMemberships { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<Setting>? Settings { get; set; }
    }
}
