using System;
using System.Collections.Generic;
using System.Text;

namespace Taxi.Common.Helpers
{
    public interface IRegexHelper
    {
        bool IsValidEmail(string emailaddress);
    }
}
