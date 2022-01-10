using Bogus;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleRegistrationService.Models;

namespace VehicleRegistrationService.Repositories
{
    public class InMemoryVehicleInfoRepository : IVehicleInfoRepository
    {
        private readonly string[] _vehicleBrands = new string[] {
            "Mercedes", "Toyota", "Audi", "Volkswagen", "Seat", "Renault", "Skoda",
            "Kia", "Citroën", "Suzuki", "Mitsubishi", "Fiat", "Opel" };

        private Dictionary<string, string[]> _models = new Dictionary<string, string[]>
        {
            { "Mercedes", new string[] { "A Class", "B Class", "C Class", "E Class", "SLS", "SLK" } },
            { "Toyota", new string[] { "Yaris", "Avensis", "Rav 4", "Prius", "Celica" } },
            { "Audi", new string[] { "A3", "A4", "A6", "A8", "Q5", "Q7" } },
            { "Volkswagen", new string[] { "Golf", "Pasat", "Tiguan", "Caddy" } },
            { "Seat", new string[] { "Leon", "Arona", "Ibiza", "Alhambra" } },
            { "Renault", new string[] { "Megane", "Clio", "Twingo", "Scenic", "Captur" } },
            { "Skoda", new string[] { "Octavia", "Fabia", "Superb", "Karoq", "Kodiaq" } },
            { "Kia", new string[] { "Picanto", "Rio", "Ceed", "XCeed", "Niro", "Sportage" } },
            { "Citroën", new string[] { "C1", "C2", "C3", "C4", "C4 Cactus", "Berlingo" } },
            { "Suzuki", new string[] { "Ignis", "Swift", "Vitara", "S-Cross", "Swace", "Jimny" } },
            { "Mitsubishi", new string[] { "Space Star", "ASX", "Eclipse Cross", "Outlander PHEV" } },
            { "Ford", new string[] { "Focus", "Ka", "C-Max", "Fusion", "Fiesta", "Mondeo", "Kuga" } },
            { "BMW", new string[] { "1 Serie", "2 Serie", "3 Serie", "5 Serie", "7 Serie", "X5" } },
            { "Fiat", new string[] { "500", "Panda", "Punto", "Tipo", "Multipla" } },
            { "Opel", new string[] { "Karl", "Corsa", "Astra", "Crossland X", "Insignia" } }
        };

        public async Task<VehicleInfo> GetVehicleInfoAsync(string licenseNumber)
        {
            
            var faker = new Faker<VehicleInfo>()
                .RuleFor(x=>x.OwnerName, f=>f.Person.FullName)
                .RuleFor(x=>x.OwnerEmail, f=>f.Person.Email)
                .RuleFor(x=>x.Brand,f=>f.PickRandom(_vehicleBrands))
                .RuleFor(x => x.Model, (f, m) => {
                    return f.PickRandom(_models[m.Brand]);
                })
                .RuleFor(x=>x.VehicleId, f=> licenseNumber);

            var result = faker.Generate();
            await Task.Delay(new Faker().Random.Number(5, 200));
            return result;
        }
    }
}
