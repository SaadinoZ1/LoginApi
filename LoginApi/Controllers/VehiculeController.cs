using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class VehiculeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public VehiculeController(ApplicationDbContext context)
        { _context = context; }


            [HttpPost]
            public async Task<ActionResult<VehiculeDto>> AddVehicule(VehiculeDto vehiculeDto)
            {
                var vehicule = vehiculeDto.Adapt<Vehicule>();
                _context.Vehicules.Add(vehicule);
                await _context.SaveChangesAsync();
                return Ok(vehiculeDto);
            }
        [HttpGet]
            public async Task<ActionResult<List<VehiculeDto>>> GetAllVehicules()
            {
                var vehicules = await _context.Vehicules.ToListAsync();
                var VehiculesDto = vehicules.Adapt<List<VehiculeDto>>();

                return Ok(VehiculesDto);
            }
            [HttpGet("{Id}")]
            public async Task<ActionResult<VehiculeDto>> GetVehicule(int Id)
            {
                var vehicules = await _context.Vehicules.FindAsync(Id);
                if (vehicules == null)
                    return NotFound("Vehicule not found ");
                var VehiculeDto = vehicules.Adapt<VehiculeDto>();
                return Ok(VehiculeDto);
            }

            [HttpPut("{id}")]
            public async Task<ActionResult<VehiculeDto>> UpdateVehicule(int id, VehiculeDto updatedVehiculeDto)
            {
                if (id != updatedVehiculeDto.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var dbVehicule = await _context.Vehicules.FindAsync(id);
                if (dbVehicule == null)
                {
                    return NotFound("Véhicule not found.");
                }

                updatedVehiculeDto.Adapt(dbVehicule);

                await _context.SaveChangesAsync();

                return Ok(updatedVehiculeDto);
            }
            [HttpDelete("{Id}")]
            public async Task<ActionResult> DeleteVehicule(int Id)
            {
                var dbVehicule = await _context.Vehicules.FindAsync(Id);
                if (dbVehicule == null)
                    return NotFound("Vehicule not found");
                _context.Vehicules.Remove(dbVehicule);
                await _context.SaveChangesAsync();


                return Ok();
            }



        }

    } 

