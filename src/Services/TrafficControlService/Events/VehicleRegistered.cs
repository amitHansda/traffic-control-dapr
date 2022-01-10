using System;

namespace TrafficControlService.Events
{
    public record VehicleRegistered
    {
        public DateTime Timestamp { get;  set; }
        public int Lane { get;  set; }
        public string LicenseNumber { get;  set; }
    }
}
