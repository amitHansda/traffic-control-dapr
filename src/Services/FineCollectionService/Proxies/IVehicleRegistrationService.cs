using FineCollectionService.Models;
using System.Threading.Tasks;

namespace FineCollectionService.Proxies
{
    public interface IVehicleRegistrationService
    {
        Task<VehicleInfo> GetVehicleInfo(string licenseNumber);
    }
}
