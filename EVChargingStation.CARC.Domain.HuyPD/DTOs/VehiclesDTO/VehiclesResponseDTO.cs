using EVChargingStation.CARC.Domain.HuyPD.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVChargingStation.CARC.Domain.HuyPD.DTOs.VehiclesDTO
{
    public class VehiclesResponseDTO
    {
        [Required]        
        public string Make { get; set; } = string.Empty;

        [Required]        
        public string Model { get; set; } = string.Empty;

        public int? Year { get; set; }
        
        public string? LicensePlate { get; set; }

        [Required]
        public ConnectorType ConnectorType { get; set; }
    }
}
