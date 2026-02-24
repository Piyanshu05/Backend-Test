using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupporTicketManagement.Models
{
    [Table("ticket_status_logs")]
    public class TicketStatusLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("ticket_id")]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; } = null!;
        [Required]
        [Column("old_status")]
        public string OldStatus { get; set; } = string.Empty;
        [Required]
        [Column("new_status")]
        public string NewStatus { get; set; } = string.Empty;
        [Column("changed_by")]
        public int ChangedBy { get; set; }
        [ForeignKey("ChangedBy")]
        public User ChangedByUser { get; set; } = null!;
        [Column("changed_at")]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
