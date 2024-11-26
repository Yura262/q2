using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Черга.Models
{
    public class Queue
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } = null!;
        public DateTime EnabledDate { get; set; }
        public QueueType Type { get; set; }
        public int GroupId { get; set; } // Foreign Key to Group
        [ForeignKey("GroupId")]
        public Group? Group { get; set; }
        public ICollection<QueueEntry> Entries { get; set; } = new List<QueueEntry>();
    }
    //public class User
    //{

    //    public string Id { get; set; } // Primary Key
    //    public string Username { get; set; } = null!;
    //    public string Email { get; set; } = null!;
    //    public ICollection<GroupMembership>? GroupMemberships { get; set; }
    //}
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }



        public ICollection<GroupMembership>? GroupMemberships { get; set; }
    }
    public class QueueEntry
    {
        public int Id { get; set; } // Primary Key

        public int QueueId { get; set; } // Foreign Key to Queue
        public string UserId { get; set; } // Foreign Key to User
        public DateTime JoinDateTime { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("QueueId")]
        public Queue Queue { get; set; }
    }
    public enum QueueType
    {
        [Display(Name ="По часу приєднання")]
        OrderedByJoinDate=0,
        [Display(Name = "Випадковим чином")]
        Random=1
        // Future types like MiniGames can be added here
    }

}
