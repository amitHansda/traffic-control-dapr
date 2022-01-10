using Dapr.Actors.Runtime;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TrafficControlService.Domain;
using TrafficControlService.Events;
using TrafficControlService.Models;

namespace TrafficControlService.Actors
{
    public class VehicleActor : Actor, IVehicleActor, IRemindable
    {
        private readonly DaprClient _daprClient;
        private readonly ISpeedingViolationCalculator _speedingViolationCalculator;
        private readonly string _roadId;

        public VehicleActor(ActorHost actorHost, DaprClient daprClient, ISpeedingViolationCalculator speedingViolationCalculator) : base(actorHost)
        {
            _daprClient = daprClient;
            _speedingViolationCalculator = speedingViolationCalculator;
            _roadId = _speedingViolationCalculator.GetRoadId();
        }


        public async Task RegisterEntryAsync(VehicleRegistered msg)
        {
            Logger.LogInformation($"ENTRY detected in lane {msg.Lane} at " +
               $"{msg.Timestamp.ToString("hh:mm:ss")} " +
               $"of vehicle with license-number {msg.LicenseNumber}.");

            // store vehicle state
            var vehicleState = new VehicleState() { LicenseNumber = msg.LicenseNumber, EntryTimestamp = msg.Timestamp };
            await this.StateManager.SetStateAsync("VehicleState", vehicleState);

            // register a reminder for cars that enter but don't exit within 20 seconds
            // (they might have broken down and need road assistence)
            await RegisterReminderAsync("VehicleLost", null,
                TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));
        }

        public async Task RegisterExitAsync(VehicleRegistered msg)
        {
            try
            {
                Logger.LogInformation($"EXIT detected in lane {msg.Lane} at { msg.Timestamp.ToString("hh:mm:ss")} of vehicle with license number { msg.LicenseNumber}");

                //remove lost vehicle timer
                await UnregisterReminderAsync("VehicleLost");

                //get vehicle state
                var vehicleState = await this.StateManager.GetStateAsync<VehicleState>("VehicleState");
                vehicleState = vehicleState with { ExitTimestamp = msg.Timestamp };
                await this.StateManager.SaveStateAsync();

                //handle possible speeding violation
                int violation = _speedingViolationCalculator.DetermineSpeedingViolationInKmh
                    (vehicleState.EntryTimestamp, vehicleState.ExitTimestamp.Value);
                if (violation > 0)
                {
                    Logger.LogInformation("Speeding violation detected ({violation} KMh) of vehicle with number plate {licenseNumber}",
                        violation, vehicleState.LicenseNumber);

                    var speedingViolation = new SpeedingViolation
                    {
                        VehicleId = msg.LicenseNumber,
                        RoadId = _roadId,
                        ViolationInKmh = violation,
                        Timestamp = msg.Timestamp
                    };

                    await _daprClient.PublishEventAsync("pubsub", "speedingviolations", speedingViolation);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in registerExit of {vehicleNumber}", msg.LicenseNumber);
            }
        }
        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == "VehicleLost")
            {
                // remove lost vehicle timer
                await UnregisterReminderAsync("VehicleLost");

                var vehicleState = await this.StateManager.GetStateAsync<VehicleState>("VehicleState");

                Logger.LogInformation($"Lost track of vehicle with license-number {vehicleState.LicenseNumber}. " +
                    "Sending road-assistence.");

                // send road assistence ...
            }
        }
    }
}
