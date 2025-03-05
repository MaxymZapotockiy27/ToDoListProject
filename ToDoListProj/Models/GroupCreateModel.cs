namespace ToDoListProj.Models
{
    public class GroupCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile file { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<GroupTaskDto> GroupTasks { get; set; }
        public List<GroupMemberDto> GroupMembers { get; set; }
    }
}
