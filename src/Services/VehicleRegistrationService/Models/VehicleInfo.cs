namespace VehicleRegistrationService.Models
{
    public record VehicleInfo
    {
        public string VehicleId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
    }
}
