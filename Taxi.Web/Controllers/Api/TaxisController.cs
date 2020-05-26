using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;

namespace Taxi.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxisController : ControllerBase
    {
        private readonly DataContext _context;

        public TaxisController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Taxis
        [HttpGet]
        public IEnumerable<TaxiEntity> GetTaxis()
        {
            return _context.Taxis;
        }

        // GET: api/Taxis/5
        [HttpGet("{plaque}")]
        public async Task<IActionResult> GetTaxiEntity([FromRoute] string plaque)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxiEntity = await _context.Taxis
                                                .Include(t => t.Trips )
                                                .FirstOrDefaultAsync( t => t.Plaque ==plaque);

            if (taxiEntity == null)
            {
                return NotFound();
            }

            return Ok(taxiEntity);
        }

       
    }
}