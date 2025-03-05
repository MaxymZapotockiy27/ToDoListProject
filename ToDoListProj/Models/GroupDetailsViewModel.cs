namespace ToDoListProj.Models
{
    public class GroupDetailsViewModel
    {
        public GroupTask Group { get; set; } // Вся інформація про групу
        public List<ApplicationUser> Members { get; set; } // Всі учасники групи
        public List<TaskItem> Tasks { get; set; } // Усі таски групи
    }


}
