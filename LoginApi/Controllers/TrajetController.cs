using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TrajetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TrajetController(ApplicationDbContext context)
        { _context = context; }

        [HttpPost, Authorize(Roles = "Manager Exploitation")]
        public async Task<ActionResult<TrajetDto>> AddTrajet(TrajetDto trajetDto)
        {
            var trajet = trajetDto.Adapt<Trajet>();
            _context.Trajets.Add(trajet);
            await _context.SaveChangesAsync();
            return Ok(trajetDto);
        }

        [HttpGet,Authorize(Roles = "AgentExploitation, Manager Exploitation")]
        public async Task<ActionResult<List<TrajetDto>>> GetAllTrajets()
        {
            var trajets = await _context.Trajets.ToListAsync();
            var trajetsDto = trajets.Adapt<List<TrajetDto>>();

            return Ok(trajetsDto);
        }
        [HttpGet("{Id}"), Authorize(Roles = "AgentCommercial, Manager Commercial")]
        public async Task<ActionResult<TrajetDto>> GetTrajet(int Id)
        {
            var trajets = await _context.Trajets.FindAsync(Id);
            if (trajets == null)
                return NotFound("Trajet not found ");
            var trajetsDto = trajets.Adapt<TrajetDto>();
            return Ok(trajetsDto);
        }
        [HttpPut("{id}"),Authorize(Roles = "AgentCommercial")]
        public async Task<ActionResult<TrajetDto>> UpdateTrajet(int id, TrajetDto updatedTrajetDto)
        {
            if (id != updatedTrajetDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var dbTrajet = await _context.Trajets.FindAsync(id);
            if (dbTrajet == null)
            {
                return NotFound("Trajet not found.");
            }

            updatedTrajetDto.Adapt(dbTrajet);

            await _context.SaveChangesAsync();

            return Ok(updatedTrajetDto);
        }
        [HttpDelete("{Id}"),Authorize(Roles = "Manager Exploitation")]
        public async Task<ActionResult> DeleteTrajet(int Id)
        {
            var dbTrajet = await _context.Trajets.FindAsync(Id);
            if (dbTrajet == null)
                return NotFound("Trajet not found");
            _context.Trajets.Remove(dbTrajet);
            await _context.SaveChangesAsync();


            return Ok();
        }
    }
}
