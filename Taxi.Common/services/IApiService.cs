using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Taxi.Common.Models;

namespace Taxi.Common
{
    public interface IApiService
    {
        Task<Response> GetTaxiAsync(string plaque, string urlBase, string servicePrefix, string controller);
        Task<bool> CheckConnectionAsync(string url);
        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);

        Task<Response> GetUserByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, EmailRequest request);

    }
}
