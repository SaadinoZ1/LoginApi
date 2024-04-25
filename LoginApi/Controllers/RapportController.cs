using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RapportController : ControllerBase
    {
        
            private readonly ApplicationDbContext _context;
            public RapportController(ApplicationDbContext context)
            { _context = context; }

            [HttpPost]
            public async Task<ActionResult<RapportDto>> AddRapport(RapportDto rapportDto)
            {
                var rapport = rapportDto.Adapt<Rapport>();
                _context.Rapports.Add(rapport);
                await _context.SaveChangesAsync();
                return Ok(rapportDto);
            }

            [HttpGet]
            public async Task<ActionResult<List<RapportDto>>> GetAllRapports()
            {
                var rapports = await _context.Rapports.ToListAsync();
                var rapportsDto = rapports.Adapt<List<RapportDto>>();

                return Ok(rapportsDto);
            }

        [HttpGet("{Id}")]
        public async Task<ActionResult<RapportDto>> GetRapport(int Id)
        {
            var rapports = await _context.Rapports.FindAsync(Id);
            if (rapports == null)
                return NotFound("Rapport not found ");
            var rapportDto = rapports.Adapt<RapportDto>();
            return Ok(rapportDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RapportDto>> UpdateRapport(int id, RapportDto updatedRapportDto)
        {
            if (id != updatedRapportDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var dbRapport = await _context.Rapports.FindAsync(id);
            if (dbRapport == null)
            {
                return NotFound("Rapport not found.");
            }

            updatedRapportDto.Adapt(dbRapport);

            await _context.SaveChangesAsync();

            return Ok(updatedRapportDto);
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteRapport(int Id)
        {
            var dbRapport = await _context.Rapports.FindAsync(Id);
            if (dbRapport == null)
                return NotFound("Rapport not found");
            _context.Rapports.Remove(dbRapport);
            await _context.SaveChangesAsync();


            return Ok();
        }

    }
}
