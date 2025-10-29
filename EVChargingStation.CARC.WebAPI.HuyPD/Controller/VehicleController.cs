using EVChargingStation.CARC.Application.HuyPD.Services;
using EVChargingStation.CARC.Domain.HuyPD.DTOs.VehiclesDTO;
using EVChargingStation.CARC.Domain.HuyPD.Enums;
using EVChargingStation.CARC.Infrastructure.HuyPD.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace EVChargingStation.CARC.WebAPI.HuyPD.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        public IVehicleHuyPDService vehicleHuyPDService;

        public VehicleController(IVehicleHuyPDService vehicleHuyPDService)
        {
            this.vehicleHuyPDService = vehicleHuyPDService;
        }
        [HttpGet]        
        public async Task<Pagination<VehiclesResponseDTO>> getAllVehicleAsync(int page, int pageSize, string? model, string? brand, ConnectorType? connectorType)
        {
           return await vehicleHuyPDService.getAllVehicleAsync(page, pageSize, model, brand, connectorType);            
        }
        [HttpGet("/{guid}")]        
        public async Task<ActionResult<VehiclesResponseDTO>> getVehicleByIdAsync(Guid guid)
        {
            var vehicle = await vehicleHuyPDService.getVehicleByIdAsync(guid);
            if (vehicle == null)
                return NotFound(new { message = $"Vehicle with ID {guid} not found." });

            return Ok(vehicle);
        }
        [HttpPost]        
        public async Task<IActionResult> AddVehicleAsync([FromBody]VehiclesResponseDTO vehicleDTO)
        {
            if (vehicleDTO == null)
                return BadRequest(new { message = "Vehicle data is required." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { message = "User not authenticated." });

            var result = await vehicleHuyPDService.AddVehicleAsync(vehicleDTO, Guid.Parse(userId));

            return CreatedAtAction(
                nameof(getVehicleByIdAsync),                
                new { message = "Vehicle added successfully."}
            );
        }
        [HttpPut("/{guid}")]
        public async Task<IActionResult> UpdateVehicleAsync([FromBody]VehiclesResponseDTO vehicleDTO, Guid guid)
        {
            if (vehicleDTO == null)
                return BadRequest(new { message = "Vehicle data is required." });

            var updated = await vehicleHuyPDService.UpdateVehicleAsync(vehicleDTO, guid);
            if (!updated)
                return NotFound(new { message = $"Vehicle with ID {guid} not found." });

            return Ok(new { message = "Vehicle updated successfully." });
        }
        [HttpDelete("/{guid}")]
        public async Task<IActionResult> DeleteVehicleAsync(Guid guid)
        {
            var deleted = await vehicleHuyPDService.DeleteVehicleAsync(guid);
            if (!deleted)
                return NotFound(new { message = $"Vehicle with ID {guid} not found." });

            return NoContent();
        }
    }
}
