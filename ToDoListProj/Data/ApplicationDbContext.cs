using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoListProj.Models;

namespace ToDoListProj.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<GroupTask> GroupTasks { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Friendship> Friendships { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Визначення зв'язків між сутностями
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.GroupTaskId, gm.UserId }); // Задаємо комбінований ключ для GroupMember

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships) // Це залежить від того, чи має ApplicationUser колекцію GroupMemberships
                .HasForeignKey(gm => gm.UserId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.GroupTask)
                .WithMany(gt => gt.Members) // Колекція членів у GroupTask
                .HasForeignKey(gm => gm.GroupTaskId); // Виправлення: правильне ім'я зовнішнього ключа

            modelBuilder.Entity<Friendship>()
                .HasKey(f => f.Id); // визначення первинного ключа

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User) // Зв'язок з ApplicationUser (перший користувач)
                .WithMany() // Користувач може мати багато друзів
                .HasForeignKey(f => f.UserId) // Зовнішній ключ для UserId
                .OnDelete(DeleteBehavior.Restrict); // При видаленні користувача не видаляти дружбу

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend) // Зв'язок з ApplicationUser (другий користувач)
                .WithMany() // Друг може мати багато користувачів
                .HasForeignKey(f => f.FriendId) // Зовнішній ключ для FriendId
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
