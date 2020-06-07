using System;
using System.Collections.Generic;
using System.Text;
using Taxi.Common.Models;

namespace Taxi.Prism.Helpers
{
    public class CombosHelper
    {
        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role { Id = 1, Name = "User" },
                new Role { Id = 2, Name = "Driver" }
            };
        }
    }
}
