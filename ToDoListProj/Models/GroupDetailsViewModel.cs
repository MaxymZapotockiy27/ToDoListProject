namespace ToDoListProj.Models
{
    public class GroupDetailsViewModel
    {
        public GroupTask Group { get; set; }  
        public List<ApplicationUser> Members { get; set; }  
        public List<TaskItem> Tasks { get; set; } 
    }


}
