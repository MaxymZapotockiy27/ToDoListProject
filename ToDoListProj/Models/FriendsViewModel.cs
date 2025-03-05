namespace ToDoListProj.Models
{
    public class FriendsViewModel
    {
        public string UserId { get; set; }  // Ідентифікатор користувача
        public string FullName { get; set; } // Повне ім'я користувача
        public int FriendshipId { get; set; } // Ідентифікатор дружнього зв'язку
        public string Id { get; set; } // Ідентифікатор друга (можливо, користувача)
        public string? AvatarUrl { get; set; }
    }
}
