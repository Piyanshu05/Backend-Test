using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupporTicketManagement.Data;
using SupporTicketManagement.DTOs;
using SupporTicketManagement.Models;

namespace SupporTicketManagement.Controllers
{
    [ApiController]
    [Tags("Comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CommentsController(AppDbContext db)
        {
            _db = db;
        }
        private int getCurrentUsrId()
        {
            return int.Parse(User.FindFirst("userId")!.Value);
        }
        private string getCurrentRole()
        {
            return User.FindFirst("role")!.Value;
        }

        [HttpPost("tickets/{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CommentDTO dto)
        {
            var rl = getCurrentRole();
            var usrId = getCurrentUsrId();
            var tickt = await _db.Tickets.FindAsync(id);
            if (tickt == null)
                return NotFound(new { message = "ticket not fond" });
            if (rl == "SUPPORT" && tickt.AssignedTo != usrId)
                return StatusCode(403, new { message = "support can only commet on asigned tickets" });
            if (rl == "USER" && tickt.CreatedBy != usrId)
                return StatusCode(403, new { message = "user can only commet on own tickets" });
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return BadRequest(new { message = "comment is requird" });
            var cmnt = new TicketComment
            {
                TicketId = id,
                UserId = usrId,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            _db.TicketComments.Add(cmnt);
            await _db.SaveChangesAsync();
            var savd = await _db.TicketComments
                .Include(c => c.User).ThenInclude(u => u.Role)
                .FirstAsync(c => c.Id == cmnt.Id);
            return StatusCode(201, mapCmntResponce(savd));
        }

        [HttpGet("tickets/{id}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var rl = getCurrentRole();
            var usrId = getCurrentUsrId();
            var tickt = await _db.Tickets.FindAsync(id);
            if (tickt == null)
                return NotFound(new { message = "ticket not fond" });
            if (rl == "SUPPORT" && tickt.AssignedTo != usrId)
                return StatusCode(403, new { message = "support can only veiw comments on asigned tickets" });
            if (rl == "USER" && tickt.CreatedBy != usrId)
                return StatusCode(403, new { message = "user can only veiw comments on own tickets" });
            var cmnts = await _db.TicketComments
                .Where(c => c.TicketId == id)
                .Include(c => c.User).ThenInclude(u => u.Role)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
            var reslt = cmnts.Select(c => mapCmntResponce(c)).ToList();
            return Ok(reslt);
        }

        [HttpPatch("comments/{id}")]
        public async Task<IActionResult> EditComment(int id, [FromBody] CommentDTO dto)
        {
            var rl = getCurrentRole();
            var usrId = getCurrentUsrId();
            var cmnt = await _db.TicketComments
                .Include(c => c.User).ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cmnt == null)
                return NotFound(new { message = "comment not fond" });
            if (rl != "MANAGER" && cmnt.UserId != usrId)
                return StatusCode(403, new { message = "only the auther or manager can edit this comment" });
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return BadRequest(new { message = "comment is requird" });
            cmnt.Comment = dto.Comment;
            await _db.SaveChangesAsync();
            return Ok(mapCmntResponce(cmnt));
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var rl = getCurrentRole();
            var usrId = getCurrentUsrId();
            var cmnt = await _db.TicketComments.FindAsync(id);
            if (cmnt == null)
                return NotFound(new { message = "comment not fond" });
            if (rl != "MANAGER" && cmnt.UserId != usrId)
                return StatusCode(403, new { message = "only the auther or manager can delet this comment" });
            _db.TicketComments.Remove(cmnt);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private CommentResponceDTO mapCmntResponce(TicketComment c)
        {
            return new CommentResponceDTO
            {
                Id = c.Id,
                Comment = c.Comment,
                User = new UserResponceDTO
                {
                    Id = c.User.Id,
                    Name = c.User.Name,
                    Email = c.User.Email,
                    Role = new RoleResponceDTO { Id = c.User.Role.Id, Name = c.User.Role.Name },
                    CreatedAt = c.User.CreatedAt
                },
                CreatedAt = c.CreatedAt
            };
        }
    }
}
