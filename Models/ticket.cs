using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupporTicketManagement.Models
{
    [Table("tickets")]
    public class Ticket
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Column("status")]
        public string Status { get; set; } = "OPEN";
        [Required]
        [Column("priority")]
        public string Priority { get; set; } = "MEDIUM";
        [Column("created_by")]
        public int CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User Creator { get; set; } = null!;
        [Column("assigned_to")]
        public int? AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public User? Assignee { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
        public ICollection<TicketStatusLog> StatusLogs { get; set; } = new List<TicketStatusLog>();
    }
}
