using System.ComponentModel.DataAnnotations;

namespace SupporTicketManagement.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
    public class CreateUserDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }
    public class CreateTicketDTO
    {
        [Required]
        [MinLength(5)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(10)]
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "MEDIUM";
    }
    public class AssignDTO
    {
        [Required]
        public int UserId { get; set; }
    }
    public class UpdateStatusDTO
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
    public class CommentDTO
    {
        [Required]
        public string Comment { get; set; } = string.Empty;
    }
    public class RoleResponceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class UserResponceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RoleResponceDTO Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
    public class TicketResponceDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public UserResponceDTO CreatedByUser { get; set; } = null!;
        public UserResponceDTO? AssignedToUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CommentResponceDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public UserResponceDTO User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
    public class AuthResponceDTO
    {
        public string Token { get; set; } = string.Empty;
    }
}
