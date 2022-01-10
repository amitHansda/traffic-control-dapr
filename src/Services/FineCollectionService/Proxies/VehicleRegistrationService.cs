using Dapr.Client;
using FineCollectionService.Models;
using System.Threading.Tasks;
using System.Net.Http;
namespace FineCollectionService.Proxies
{

    public class VehicleRegistrationService : IVehicleRegistrationService
    {
        private readonly DaprClient _daprclient;

        public VehicleRegistrationService(DaprClient daprclient)
        {
            this._daprclient = daprclient;
        }

        public async Task<VehicleInfo> GetVehicleInfo(string licenseNumber)
        {
            return await _daprclient.InvokeMethodAsync<VehicleInfo>(HttpMethod.Get,"vehicleregistrationservice",$"vehicleinfo/{licenseNumber}");            
        }
    }
}
