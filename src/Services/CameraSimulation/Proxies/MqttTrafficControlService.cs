using CameraSimulation.Events;
using System.Net.Mqtt;
using System.Text;
using System.Text.Json;

namespace CameraSimulation.Proxies
{
    public class MqttTrafficControlService : ITrafficControlService
    {
        private readonly IMqttClient _client;
        public MqttTrafficControlService(int camNumber)
        {
            var mqttHost = Environment.GetEnvironmentVariable("MQTT_HOST") ?? "localhost";
            _client = MqttClient.CreateAsync(mqttHost, 1883).Result;
            var sessionState = _client.ConnectAsync(
                new MqttClientCredentials(clientId: $"camerasim{camNumber}")).Result;
        }
        public async Task SendVehicleEntryAsync(VehicleRegistered @event)
        {
            var eventJson = JsonSerializer.Serialize(@event);
            var message = new MqttApplicationMessage("trafficcontrol/entrycam", Encoding.UTF8.GetBytes(eventJson));
            await _client.PublishAsync(message, MqttQualityOfService.AtMostOnce);
        }

        public async Task SendVehicleExitAsync(VehicleRegistered @event)
        {
            var eventJson = JsonSerializer.Serialize(@event);
            var message = new MqttApplicationMessage("trafficcontrol/exitcam", Encoding.UTF8.GetBytes(eventJson));
            await _client.PublishAsync(message, MqttQualityOfService.AtMostOnce);

        }
    }
}
