namespace ToDoListProj.Models
{
    public class GroupTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }  // <-- Nullable
        public bool IsCompleted { get; set; }
        public string UserId { get; set; }
        public int? ProjectId { get; set; }
    }


}
