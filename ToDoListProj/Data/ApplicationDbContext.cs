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
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                 foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTimeOffset)
                                 || p.PropertyType == typeof(DateTimeOffset?));

                    foreach (var property in properties)
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name)
                            .HasConversion<string>();
                    }
                }

                 modelBuilder.Entity<ApplicationUser>().Property(u => u.UserName).HasMaxLength(256);
                modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).HasMaxLength(256);
            }
            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new { gm.GroupTaskId, gm.UserId }); 

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)  
                .HasForeignKey(gm => gm.UserId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.GroupTask)
                .WithMany(gt => gt.Members)  
                .HasForeignKey(gm => gm.GroupTaskId); 

            modelBuilder.Entity<Friendship>()
                .HasKey(f => f.Id); 

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)  
                .WithMany() 
                .HasForeignKey(f => f.UserId) 
                .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)  
                .WithMany()  
                .HasForeignKey(f => f.FriendId)  
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
