using System;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrafficControlService.Actors;
using TrafficControlService.Domain;
using TrafficControlService.Events;
using TrafficControlService.Repositories;

namespace TrafficControlService.Controllers
{
    [ApiController]
    [Route("")]
    public class TrafficController : ControllerBase
    {
        private readonly ILogger<TrafficController> _logger;
        private readonly IVehicleStateRepository _vehicleStateRepository;
        private readonly ISpeedingViolationCalculator _speedingViolationCalculator;

        public TrafficController(ILogger<TrafficController> logger, IVehicleStateRepository vehicleStateRepository, ISpeedingViolationCalculator speedingViolationCalculator)
        {
            this._logger = logger;
            this._vehicleStateRepository = vehicleStateRepository;
            this._speedingViolationCalculator = speedingViolationCalculator;
        }

        [HttpPost("entrycam")]
        public async Task<ActionResult> VehicleEntryAsync([FromBody]VehicleRegistered @event)
        {
            try
            {
                var actorId = new ActorId(@event.LicenseNumber);
                var proxy = ActorProxy.Create<IVehicleActor>(actorId, nameof(VehicleActor));
                await proxy.RegisterEntryAsync(@event);
                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        [HttpPost("exitcam")]
        public async Task<IActionResult> VehicleExitAsync([FromBody]VehicleRegistered @event)
        {
            try
            {
                var actorId = new ActorId(@event.LicenseNumber);
                var proxy = ActorProxy.Create<IVehicleActor>(actorId, nameof(VehicleActor));
                await proxy.RegisterExitAsync(@event);
                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }
    }
}
