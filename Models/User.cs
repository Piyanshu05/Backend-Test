using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupporTicketManagement.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Column("password")]
        public string Password { get; set; } = string.Empty;
        [Column("role_id")]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
