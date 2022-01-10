using Dapr;
using Dapr.Client;
using FineCollectionService.DomainServices;
using FineCollectionService.Helpers;
using FineCollectionService.Models;
using FineCollectionService.Proxies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FineCollectionService.Controllers
{
    [ApiController]
    [Route("")]
    public class CollectionController : ControllerBase
    {
        private static string? _fineCalculatorLicenseKey = null;
        private readonly ILogger<CollectionController> _logger;
        private readonly IFineCalculator _fineCalculator;
        private readonly IVehicleRegistrationService _vehicleRegistrationService;
        private readonly DaprClient _daprClient;

        public CollectionController(ILogger<CollectionController> logger,
            IFineCalculator fineCalculator,
            IVehicleRegistrationService vehicleRegistrationService,
            DaprClient daprClient)
        {
            this._logger = logger;
            this._fineCalculator = fineCalculator;
            this._vehicleRegistrationService = vehicleRegistrationService;
            this._daprClient = daprClient;


            if (_fineCalculatorLicenseKey == null)
            {
                bool runningInK8s = Convert.ToBoolean(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") ?? "false");
                var metadata = new Dictionary<string, string> { { "namespace", "dapr-trafficcontrol" } };
                if (runningInK8s)
                {
                    var k8sSecrets = daprClient.GetSecretAsync(
                        "kubernetes", "trafficcontrol-secrets", metadata).Result;
                    _fineCalculatorLicenseKey = k8sSecrets["finecalculator.licensekey"];
                }
                else
                {
                    var secrets = daprClient.GetSecretAsync(
                        "trafficcontrol-secrets", "finecalculator.licensekey", metadata).Result;
                    _fineCalculatorLicenseKey = secrets["finecalculator.licensekey"];
                }
            }
        }


        [Topic("pubsub", "speedingviolations")]
        [Route("collectfine")]
        [HttpPost()]
        public async Task<ActionResult> CollectFine(SpeedingViolation speedingViolation)
        {
            decimal fine = _fineCalculator.CalculateFine(_fineCalculatorLicenseKey, speedingViolation.ViolationInKmh);
            //get owner info
            var vehicleInfo = await _vehicleRegistrationService.GetVehicleInfo(speedingViolation.VehicleId);

            string fineString = fine == 0 ? "TBD by prosecutor" : $"{fine} USD";
            _logger.LogInformation($"Sent speeding ticket to {vehicleInfo.OwnerName}. " +
           $"Road: {speedingViolation.RoadId}, Licensenumber: {speedingViolation.VehicleId}, " +
           $"Vehicle: {vehicleInfo.Brand} {vehicleInfo.Model}, " +
           $"Violation: {speedingViolation.ViolationInKmh} Km/h, Fine: {fineString}, " +
           $"On: {speedingViolation.Timestamp.ToString("dd-MM-yyyy")} " +
           $"at {speedingViolation.Timestamp.ToString("hh:mm:ss")}.");

            var body = EmailUtils.CreateEmailBody(speedingViolation, vehicleInfo, fineString);
            var metadata = new Dictionary<string, string>
            {
                ["emailFrom"] = "noreply@cfca.gov",
                ["emailTo"] = vehicleInfo.OwnerEmail,
                ["subject"] = $"Speeding violation on the {speedingViolation.RoadId}"
            };
            await _daprClient.InvokeBindingAsync("sendmail", "create", body, metadata);
            return Ok();
        }
    }

}
