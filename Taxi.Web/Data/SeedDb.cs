using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Web.Data.Entities;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            this._context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckTaxisAsync();
        }


        private async Task CheckTaxisAsync()
        {
            if (!_context.Taxis.Any())
            {
                _context.Taxis.Add(new TaxiEntity
                {
                    Plaque = "ETP123",
                    Trips = new List<TripEntity> {
                       new TripEntity {
                       StartDate = DateTime.UtcNow,
                       EndDate = DateTime.UtcNow.AddMinutes(30),
                       Qualification =4.6f,
                       Source ="Monte Verde",
                       Target="Robledo",
                       Remarks ="Buena honda"
                    },
                    new TripEntity{
                       StartDate = DateTime.UtcNow,
                       EndDate = DateTime.UtcNow.AddMinutes(60),
                       Qualification =4.6f,
                       Source ="Monte Rico",
                       Target="Rumbea",
                       Remarks ="mala cosa"
                    }}
                });

                _context.Taxis.Add(new TaxiEntity
                {
                    Plaque = "FTR123",
                    Trips = new List<TripEntity> { new TripEntity {
                      EndDate = DateTime.UtcNow,
                      StartDate = DateTime.UtcNow.AddMinutes(30),
                      Qualification=3.5f,
                      Source = "las colinas",
                      Target = "El cristo",
                      Remarks = "Buena la vieja"

                    },
                    new TripEntity{
                       StartDate = DateTime.UtcNow,
                       EndDate = DateTime.UtcNow.AddMinutes(80),
                       Qualification =4.6f,
                       Source ="las rocas",
                       Target="tri",
                       Remarks ="La vieja di si"
                    }}
                });
                await _context.SaveChangesAsync();
            }

        }
    }
}
