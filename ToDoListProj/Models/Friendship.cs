using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToDoListProj.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string FriendId { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Accepted, Rejected, Blocked

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("FriendId")]
        public ApplicationUser Friend { get; set; }
    }



}
