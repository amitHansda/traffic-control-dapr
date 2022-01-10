using System.Threading.Tasks;
using VehicleRegistrationService.Models;

namespace VehicleRegistrationService.Repositories
{
    public interface IVehicleInfoRepository
    {
        Task<VehicleInfo> GetVehicleInfoAsync(string licenseNumber);
    }
}
