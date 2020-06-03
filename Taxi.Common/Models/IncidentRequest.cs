using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Taxi.Common.Models
{
    public  class IncidentRequest
    {
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Remarks { get; set; }
    }
}
