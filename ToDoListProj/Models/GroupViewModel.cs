namespace ToDoListProj.Models
{
    public class GroupViewModel
    {
        public List<GroupTask> Group { get; set; }  // Groups the user is part of
        public List<GroupMember> GroupMembers { get; set; }  // All members in the user's groups
    }

}
