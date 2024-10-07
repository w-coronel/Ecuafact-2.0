using Ecuafact.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioDashboard
    { 
        public static DashboardInfo ObtenerEstadisticas(string tokenId)
        {
            var estadisticas = new DashboardInfo();

            try
            {
                var httpClient = ClientHelper.GetClient(tokenId);

                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Statistics").Result;
                
                if (response != null)
                {
                    estadisticas = response.GetContent<DashboardInfo>();
                }
            }
            catch (Exception)
            {
                // No se realiza nada.
            }

            return estadisticas;
        }
    }
}
