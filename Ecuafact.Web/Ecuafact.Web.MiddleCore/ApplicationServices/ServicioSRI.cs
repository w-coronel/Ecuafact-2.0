using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Entities.SRI;
using Ecuafact.Web.Domain.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioSRI
    { 
        public static async Task<OperationResult> ConnectAsync(string securityToken, string password)
        {
            var session = new OperationResult();

            var httpClient = ClientHelper.GetClient(securityToken, password);

            var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/SRI/Connect", new StringContent(string.Empty));

            if (response.IsSuccessStatusCode)
            {
                session = await response.GetContentAsync<OperationResult>();
            }

            return session;
        }

        public static async Task<OperationResult<SRIContribDto>> FindByRucAsync(string applicationToken, string ruc)
        {
            var session = new OperationResult<SRIContribDto>();

            var httpClient = ClientHelper.GetClient(applicationToken);
            
            var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/SRI/RUC", 
                                            new { RUC = ruc }.ToContent());

            if (response.IsSuccessStatusCode)
            {
                session = await response.GetContentAsync<OperationResult<SRIContribDto>>();
            }

            return session;
        }
    }
}
