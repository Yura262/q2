using System.Text.RegularExpressions;

namespace Черга.Models
{
    public class Queue
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public DateTime EnabledDate { get; set; }
        public QueueType Type { get; set; }
        public int GroupId { get; set; } // Foreign Key to Group
        public Group Group { get; set; }
        public ICollection<QueueEntry> Entries { get; set; } = new List<QueueEntry>();
    }
    public class User
    {
        public string Id { get; set; } // Primary Key
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<GroupMembership> GroupMemberships { get; set; }
    }
    public class Group
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string CreatorId { get; set; } // Foreign Key to User
        public User Creator { get; set; }
        public ICollection<GroupMembership> Members { get; set; }
        public ICollection<Queue> Queues { get; set; }
    }
    public class GroupMembership
    {
        public int Id { get; set; } // Primary Key
        public string UserId { get; set; } // Foreign Key to User
        public int GroupId { get; set; } // Foreign Key to Group
        public User User { get; set; }
        public Group Group { get; set; }
    }
    public class QueueEntry
    {
        public int Id { get; set; } // Primary Key
        public int QueueId { get; set; } // Foreign Key to Queue
        public string UserId { get; set; } // Foreign Key to User
        public DateTime JoinDateTime { get; set; }
        public User User { get; set; }
        public Queue Queue { get; set; }
    }
    public enum QueueType
    {
        OrderedByJoinDate,
        Random
        // Future types like MiniGames can be added here
    }

}
