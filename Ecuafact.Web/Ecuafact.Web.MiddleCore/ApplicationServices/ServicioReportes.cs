using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioReportes
    {

        public static async Task<HttpResponseMessage> ObtenerReporteAsync(string token, string report, string format, string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var client = ClientHelper.GetClient(token);
            var salesReportUrl = $"{Constants.WebApiUrl}/Reports/{format}/{report}?search={search}&startDate={startDate?.ToString("yyyy-MM-dd")}&endDate={endDate?.ToString("yyyy-MM-dd")}";
            
            return await client.GetAsync(salesReportUrl);
        }

        public static async Task<SalesReportResponse> ObtenerReporteVentas(string token, string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var client = ClientHelper.GetClient(token);
                client.Timeout = new TimeSpan(0, 5, 0);
                var salesReportUrl = $"{Constants.WebApiUrl}/Reports/Sales?search={search}&startDate={startDate?.ToString("yyyy-MM-dd")}&endDate={endDate?.ToString("yyyy-MM-dd")}";
                var response = await client.GetAsync(salesReportUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.GetContentAsync<SalesReportResponse>();

                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return new SalesReportResponse(false, HttpStatusCode.InternalServerError) { DevMessage = ex.ToString(), UserMessage = ex.Message };
            }
            
            return new SalesReportResponse();
        }

        public static async Task<PurchaseReportResponse> ObtenerReporteCompras(string token, string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var client = ClientHelper.GetClient(token);
                client.Timeout = new TimeSpan(0, 5, 0);
                var purchaseReportUrl = $"{Constants.WebApiUrl}/Reports/Purchases?search={search}&startDate={startDate?.ToString("yyyy-MM-dd")}&endDate={endDate?.ToString("yyyy-MM-dd")}";
                var response = await client.GetAsync(purchaseReportUrl);
                 
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.GetContentAsync<PurchaseReportResponse>();

                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return new PurchaseReportResponse(false, HttpStatusCode.InternalServerError) { DevMessage = ex.ToString(), UserMessage = ex.Message };
            }

            return new PurchaseReportResponse();
        }

        public static async Task<HttpResponseMessage> DescargarAtsAsync(string token, string format, PeriodTypeEnum periodType, int year, int month = 0, int semester = 0)
        {
            try
            {
               if(string.IsNullOrEmpty(format))
                {
                    format = "excel";
                }
                var qs = $"format={format}";
                qs += $"&periodType={periodType}";
                if (year == 0)
                {
                    year = DateTime.Now.Year;
                }
                qs += $"&year={year}";
                qs += $"&month={month}";
                qs += $"&semester={semester}";
                
                return await ClientHelper.GetClient(token).GetAsync($"{Constants.WebApiUrl}/reports/{format}/ats?{qs}");
            }
            catch (Exception ex)
            {
                ex.GetType();
                return new HttpResponseMessage();
            }


        }

        public static async Task<AtsFacturaReportResponse> ObtenerReporteFacturas(string token,  PeriodTypeEnum periodType, int year, int month = 0, int semester = 0)
        {
            try
            {
                var client = ClientHelper.GetClient(token);
                client.Timeout = new TimeSpan(0, 5, 0);
                var purchaseReportUrl = $"{Constants.WebApiUrl}/reports/ats/purchases?periodType={periodType}&year={year}&month={month}&semester={semester}";
                var response = await client.GetAsync(purchaseReportUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.GetContentAsync<AtsFacturaReportResponse>();
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return new AtsFacturaReportResponse(false, HttpStatusCode.InternalServerError) { DevMessage = ex.ToString(), UserMessage = ex.Message };
            }

            return new AtsFacturaReportResponse();
        }

        public static async Task<AtsFactura> ObtenerComprasAtsById(string token, int id, int typeIssuance)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                var serviceUrl = $"{Constants.WebApiUrl}/reports/ats/{id}/purchase?typeIssuance={typeIssuance}";
                var response = await httpClient.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.GetContent<AtsFactura>();
                    return result;
                }
            }
            catch (Exception) { }

            return default;
        }

        public static async Task<StatisticsAts> ObtenerEstadisticasComprasAts(string token, PeriodTypeEnum periodType, int year, int month = 0, int semester = 0)
        {
            try
            {
                var client = ClientHelper.GetClient(token);
                client.Timeout = new TimeSpan(0, 5, 0);
                var purchaseReportUrl = $"{Constants.WebApiUrl}/reports/statistics/ats?periodType={periodType}&year={year}&month={month}&semester={semester}";
                var response = await client.GetAsync(purchaseReportUrl);
                if (response.IsSuccessStatusCode)
                { 
                    var result = await response.GetContentAsync<OperationResult<StatisticsAts>>();
                    if (result.IsSuccess)
                    {
                        return result.Entity;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
    }
}
