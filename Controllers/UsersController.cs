using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupporTicketManagement.Data;
using SupporTicketManagement.DTOs;
using SupporTicketManagement.Models;

namespace SupporTicketManagement.Controllers
{
    [ApiController]
    [Route("users")]
    [Tags("Users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            var curRole = User.FindFirst("role")?.Value;
            if (curRole != "MANAGER")
                return StatusCode(403, new { message = "only manager can creat users" });
            if (dto == null)
                return BadRequest(new { message = "request body is requird" });
            string[] alowedRoles = { "MANAGER", "SUPPORT", "USER" };
            if (!alowedRoles.Contains(dto.Role.ToUpper()))
                return BadRequest(new { message = "role must be MANAGER, SUPPORT or USER" });
            var existingUsr = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUsr != null)
                return BadRequest(new { message = "email allready exists" });
            var rl = await _db.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role.ToUpper());
            if (rl == null)
                return BadRequest(new { message = "invalid role" });
            var hashedPwd = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUsr = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPwd,
                RoleId = rl.Id,
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(newUsr);
            await _db.SaveChangesAsync();
            await _db.Entry(newUsr).Reference(u => u.Role).LoadAsync();
            var responce = new UserResponceDTO
            {
                Id = newUsr.Id,
                Name = newUsr.Name,
                Email = newUsr.Email,
                Role = new RoleResponceDTO { Id = rl.Id, Name = rl.Name },
                CreatedAt = newUsr.CreatedAt
            };
            return StatusCode(201, responce);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var curRole = User.FindFirst("role")?.Value;
            if (curRole != "MANAGER")
                return StatusCode(403, new { message = "only manager can list users" });
            var usrs = await _db.Users.Include(u => u.Role)
                .Select(u => new UserResponceDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = new RoleResponceDTO { Id = u.Role.Id, Name = u.Role.Name },
                    CreatedAt = u.CreatedAt
                }).ToListAsync();
            return Ok(usrs);
        }
    }
}
