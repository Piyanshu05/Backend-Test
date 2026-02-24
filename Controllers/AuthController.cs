using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SupporTicketManagement.Data;
using SupporTicketManagement.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SupporTicketManagement.Controllers
{
    [ApiController]
    [Route("auth")]
    [Tags("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _confg;
        public AuthController(AppDbContext db, IConfiguration confg)
        {
            _db = db;
            _confg = confg;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { message = "email and pasword are requird" });
            var usr = await _db.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usr == null)
                return Unauthorized(new { message = "invalid credntials" });
            bool validPass = BCrypt.Net.BCrypt.Verify(dto.Password, usr.Password);
            if (!validPass)
                return Unauthorized(new { message = "invalid credntials" });
            var tokn = genrateToken(usr);
            return Ok(new AuthResponceDTO { Token = tokn });
        }

        private string genrateToken(Models.User usr)
        {
            var ky = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_confg["Jwt:Secret"]!));
            var creds = new SigningCredentials(ky, SecurityAlgorithms.HmacSha256);
            var clms = new[]
            {
                new Claim("userId", usr.Id.ToString()),
                new Claim("email", usr.Email),
                new Claim("role", usr.Role.Name)
            };
            var tokn = new JwtSecurityToken(
                issuer: _confg["Jwt:Issuer"],
                audience: _confg["Jwt:Audience"],
                claims: clms,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokn);
        }
    }
}
