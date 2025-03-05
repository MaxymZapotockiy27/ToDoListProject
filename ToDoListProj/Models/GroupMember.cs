using System.ComponentModel.DataAnnotations;

namespace ToDoListProj.Models
{
    public class GroupMember
    {
        public int GroupTaskId { get; set; }
        public GroupTask GroupTask { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }


}
