using CameraSimulation.Events;

namespace CameraSimulation.Proxies
{
    public interface ITrafficControlService
    {
        public Task SendVehicleEntryAsync(VehicleRegistered @event);
        public Task SendVehicleExitAsync(VehicleRegistered @event);
    }
}
