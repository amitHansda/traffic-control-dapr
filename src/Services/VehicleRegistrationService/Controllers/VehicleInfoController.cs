using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleRegistrationService.Models;
using VehicleRegistrationService.Repositories;

namespace VehicleRegistrationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleInfoController: ControllerBase
    {
        private readonly ILogger<VehicleInfoController> _logger;
        private readonly IVehicleInfoRepository _repository;

        public VehicleInfoController(ILogger<VehicleInfoController> logger, IVehicleInfoRepository repository)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet("{licenseNumber}")]
        public async Task<ActionResult<VehicleInfo>> GetVehicleInfoAsync(string licenseNumber)
        {
            _logger.LogInformation("Retrieving vehicle-info for {licenseNumber}", licenseNumber);
            return Ok(await _repository.GetVehicleInfoAsync(licenseNumber));
        }
    }
}
