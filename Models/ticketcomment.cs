using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupporTicketManagement.Models
{
    [Table("ticket_comments")]
    public class TicketComment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("ticket_id")]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; } = null!;
        [Column("user_id")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        [Required]
        [Column("comment")]
        public string Comment { get; set; } = string.Empty;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
