using Dapr.Actors;
using System.Threading.Tasks;
using TrafficControlService.Events;

namespace TrafficControlService.Actors
{
    public interface IVehicleActor : IActor
    {
        public Task RegisterEntryAsync(VehicleRegistered msg);
        public Task RegisterExitAsync(VehicleRegistered msg);
    }
}
