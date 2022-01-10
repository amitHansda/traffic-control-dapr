using System;

namespace TrafficControlService.Models
{
    public record SpeedingViolation
    {
        public string VehicleId { get; set; }
        public string RoadId { get; set; }
        public int ViolationInKmh { get; set; }
        public DateTime Timestamp { get; set; }
    }
}