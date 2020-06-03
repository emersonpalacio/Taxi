using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetGetComboRoles();
    }
}
