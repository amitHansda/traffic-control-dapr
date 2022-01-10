using System;

namespace TrafficControlService.Models
{
    public record VehicleState
    {
        public string LicenseNumber { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public DateTime? ExitTimestamp { get; set; }
    }
}
