using ToDoListProj.Models;

namespace ToDoListProj.ViewModel
{
    public class ProjectTaskViewModel
    {
        public string Name { get; set; } // Назва задачі
        public DateTime DueDate { get; set; } // Дата виконання
        public int ProjectId { get; set; } // До якого проекту належить
    }


}
