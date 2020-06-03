using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Taxi.Web.Data;

namespace Taxi.Web.Helpers
{
    public class CombosHelper: ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            this._context = context;
        }

        public IEnumerable<SelectListItem> GetGetComboRoles()
        {
            List<SelectListItem> lis = new List<SelectListItem> {
              new SelectListItem{Value="0",Text="[Select a rol]"},
              new SelectListItem{Value ="1", Text="Driver" },
              new SelectListItem{Value ="2", Text="User" }        
            };
            return lis;
        }
    }
}
