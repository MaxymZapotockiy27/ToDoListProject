using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListProj.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }


        [Required]
        public string SenderId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsRead { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
