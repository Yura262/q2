using System.ComponentModel.DataAnnotations.Schema;

namespace Черга.Models
{
    public class Group
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string? CreatorId { get; set; } // Foreign Key to User
        [ForeignKey("CreatorId")]
        public User? Creator { get; set; }
        public ICollection<GroupMembership>? Members { get; set; }
        public ICollection<Queue>? Queues { get; set; }
    }
    public class GroupMembership
    {
        public int Id { get; set; } // Primary Key
        public string UserId { get; set; } // Foreign Key to User

        public int GroupId { get; set; } // Foreign Key to Group
        [ForeignKey("GroupId")]
        public User User { get; set; }
        [ForeignKey("UserId")]
        public Group Group { get; set; }
        public GroupMembership(string userId, int groupId)
        {
            UserId = userId;
            //User = context

            GroupId = groupId;
        }
    }
}
