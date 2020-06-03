using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taxi.Common.Models;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]

    public class TripsController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public TripsController(DataContext context, 
                              IUserHelper userHelper, 
                              IConverterHelper converterHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostTripEntity([FromBody] TripRequest tripRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserEntity userEntity = await _userHelper.GetUserAsync(tripRequest.UserId);
            if (userEntity == null)
            {
                return BadRequest("User doesn't exists.");
            }

            TaxiEntity taxiEntity = await _context.Taxis.FirstOrDefaultAsync(t => t.Plaque == tripRequest.Plaque);
            if (taxiEntity == null)
            {
                _context.Taxis.Add(new TaxiEntity { Plaque = tripRequest.Plaque.ToUpper() });
                await _context.SaveChangesAsync();
                taxiEntity = await _context.Taxis.FirstOrDefaultAsync(t => t.Plaque == tripRequest.Plaque);
            }

            TripEntity tripEntity = new TripEntity
            {
                Source = tripRequest.Address,
                SourceLatitude = tripRequest.Latitude,
                SourceLongitude = tripRequest.Longitude,
                StartDate = DateTime.UtcNow,
                Taxi = taxiEntity,
                TripDetails = new List<TripDetailEntity>
                {
                    new TripDetailEntity
                    {
                        Date = DateTime.UtcNow,
                        Latitude = tripRequest.Latitude,
                        Longitude = tripRequest.Longitude
                    }
                },
                User = userEntity,
            };

            _context.Trips.Add(tripEntity);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToTripResponse(tripEntity));
        }

    }
}