using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupporTicketManagement.Data;
using SupporTicketManagement.DTOs;
using SupporTicketManagement.Models;

namespace SupporTicketManagement.Controllers
{
    [ApiController]
    [Route("tickets")]
    [Tags("Tickets")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private static readonly string[] validStats = { "OPEN", "IN_PROGRESS", "RESOLVED", "CLOSED" };
        private static readonly string[] validPriortys = { "LOW", "MEDIUM", "HIGH" };
        public TicketsController(AppDbContext db)
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

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDTO dto)
        {
            var rl = getCurrentRole();
            if (rl != "USER" && rl != "MANAGER")
                return StatusCode(403, new { message = "only user and manager can creat tickets" });
            if (dto.Title.Length < 5)
                return BadRequest(new { message = "title must be atleast 5 charecters" });
            if (dto.Description.Length < 10)
                return BadRequest(new { message = "descrption must be atleast 10 charecters" });
            if (!validPriortys.Contains(dto.Priority.ToUpper()))
                return BadRequest(new { message = "priorty must be LOW, MEDIUM or HIGH" });
            var usrId = getCurrentUsrId();
            var tickt = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = "OPEN",
                Priority = dto.Priority.ToUpper(),
                CreatedBy = usrId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Tickets.Add(tickt);
            await _db.SaveChangesAsync();
            var creatdTickt = await _db.Tickets
                .Include(t => t.Creator).ThenInclude(u => u.Role)
                .Include(t => t.Assignee).ThenInclude(u => u!.Role)
                .FirstAsync(t => t.Id == tickt.Id);
            return StatusCode(201, mapTicktResponce(creatdTickt));
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var rl = getCurrentRole();
            var usrId = getCurrentUsrId();
            IQueryable<Ticket> qry = _db.Tickets
                .Include(t => t.Creator).ThenInclude(u => u.Role)
                .Include(t => t.Assignee).ThenInclude(u => u!.Role);
            if (rl == "MANAGER")
            {
            }
            else if (rl == "SUPPORT")
            {
                qry = qry.Where(t => t.AssignedTo == usrId);
            }
            else
            {
                qry = qry.Where(t => t.CreatedBy == usrId);
            }
            var tickts = await qry.ToListAsync();
            var reslt = tickts.Select(t => mapTicktResponce(t)).ToList();
            return Ok(reslt);
        }

        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] AssignDTO dto)
        {
            var rl = getCurrentRole();
            if (rl != "MANAGER" && rl != "SUPPORT")
                return StatusCode(403, new { message = "only manager and support can asign tickets" });
            var tickt = await _db.Tickets
                .Include(t => t.Creator).ThenInclude(u => u.Role)
                .Include(t => t.Assignee).ThenInclude(u => u!.Role)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tickt == null)
                return NotFound(new { message = "ticket not fond" });
            var asignee = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (asignee == null)
                return BadRequest(new { message = "user not fond" });
            if (asignee.Role.Name == "USER")
                return BadRequest(new { message = "cant asign ticket to a USER role" });
            tickt.AssignedTo = dto.UserId;
            await _db.SaveChangesAsync();
            await _db.Entry(tickt).Reference(t => t.Assignee).LoadAsync();
            if (tickt.Assignee != null)
                await _db.Entry(tickt.Assignee).Reference(u => u.Role).LoadAsync();
            return Ok(mapTicktResponce(tickt));
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO dto)
        {
            var rl = getCurrentRole();
            if (rl != "MANAGER" && rl != "SUPPORT")
                return StatusCode(403, new { message = "only manager and support can update status" });
            if (!validStats.Contains(dto.Status.ToUpper()))
                return BadRequest(new { message = "status must be OPEN, IN_PROGRESS, RESOLVED or CLOSED" });
            var tickt = await _db.Tickets
                .Include(t => t.Creator).ThenInclude(u => u.Role)
                .Include(t => t.Assignee).ThenInclude(u => u!.Role)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tickt == null)
                return NotFound(new { message = "ticket not fond" });
            var newStat = dto.Status.ToUpper();
            if (!isValidTranstion(tickt.Status, newStat))
                return BadRequest(new { message = $"cant transition from {tickt.Status} to {newStat}" });
            var oldStat = tickt.Status;
            tickt.Status = newStat;
            var lg = new TicketStatusLog
            {
                TicketId = tickt.Id,
                OldStatus = oldStat,
                NewStatus = newStat,
                ChangedBy = getCurrentUsrId(),
                ChangedAt = DateTime.UtcNow
            };
            _db.TicketStatusLogs.Add(lg);
            await _db.SaveChangesAsync();
            return Ok(mapTicktResponce(tickt));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var rl = getCurrentRole();
            if (rl != "MANAGER")
                return StatusCode(403, new { message = "only manager can delet tickets" });
            var tickt = await _db.Tickets.FindAsync(id);
            if (tickt == null)
                return NotFound(new { message = "ticket not fond" });
            _db.Tickets.Remove(tickt);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private bool isValidTranstion(string current, string next)
        {
            if (current == "OPEN" && next == "IN_PROGRESS") return true;
            if (current == "IN_PROGRESS" && next == "RESOLVED") return true;
            if (current == "RESOLVED" && next == "CLOSED") return true;
            return false;
        }

        private TicketResponceDTO mapTicktResponce(Ticket t)
        {
            var resp = new TicketResponceDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
                CreatedByUser = new UserResponceDTO
                {
                    Id = t.Creator.Id,
                    Name = t.Creator.Name,
                    Email = t.Creator.Email,
                    Role = new RoleResponceDTO { Id = t.Creator.Role.Id, Name = t.Creator.Role.Name },
                    CreatedAt = t.Creator.CreatedAt
                }
            };
            if (t.Assignee != null)
            {
                resp.AssignedToUser = new UserResponceDTO
                {
                    Id = t.Assignee.Id,
                    Name = t.Assignee.Name,
                    Email = t.Assignee.Email,
                    Role = new RoleResponceDTO { Id = t.Assignee.Role.Id, Name = t.Assignee.Role.Name },
                    CreatedAt = t.Assignee.CreatedAt
                };
            }
            return resp;
        }
    }
}
