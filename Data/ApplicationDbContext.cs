using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Черга.Models;


namespace Черга.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>//, DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<Queue> Queues { get; set; }
        public DbSet<QueueEntry> QueueEntries { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure GroupMembership foreign key relationships
            modelBuilder.Entity<GroupMembership>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            modelBuilder.Entity<GroupMembership>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Group foreign key relationship
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany()
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Queue foreign key relationships
            modelBuilder.Entity<Queue>()
                .HasOne(q => q.Group)
                .WithMany(g => g.Queues)
                .HasForeignKey(q => q.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure QueueEntry foreign key relationships
            modelBuilder.Entity<QueueEntry>()
                .HasOne(qe => qe.User)
                .WithMany()
                .HasForeignKey(qe => qe.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QueueEntry>()
                .HasOne(qe => qe.Queue)
                .WithMany(q => q.Entries)
                .HasForeignKey(qe => qe.QueueId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder); // Call base for any additional configuration
        }
    }
}
