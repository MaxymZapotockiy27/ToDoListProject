using System.ComponentModel.DataAnnotations;

namespace ToDoListProj.Models
{
    public class Setting
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }

}
