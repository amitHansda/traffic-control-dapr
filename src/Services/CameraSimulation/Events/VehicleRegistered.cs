using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraSimulation.Events
{
    public record struct VehicleRegistered(int Lane, string LicenseNumber, DateTime Timestamp);
}
