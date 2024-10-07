using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Services;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public partial class ServicioGastos
    {

        public static async Task<DeductiblesReportResponse> GetExpenseReportAsync(string token, int year = 0, int month = 0)
        {
            if (year < 2010)
            {
                year = DateTime.Now.Year;
            }

            var httpClient = ClientHelper.GetClient(token);
            {
                var serviceUrl = $"{Constants.ServiceAppUrl}/documents/deductibles/report?client_token={token}&year={year}&month={month}";

                var response = await httpClient.GetAsync(serviceUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.GetContent<DeductiblesReportResponse>();

                    if (result.deductibles == null)
                    {
                        var deductibles = await GetDeduciblesAsync(token, year);
                        result.deductibles = deductibles?.deductibles ?? new List<DeductibleSum>();
                    }

                    result.deductibles = result.deductibles.OrderBy(x => x.id).ToList();

                    return await Task.FromResult(result);
                }
            }

            return new DeductiblesReportResponse() { result = new ApiResponse() { code = "400", message = "Los datos ingresados son incorrectos" } };
        }

        public static async Task<DeductibleLimitResponse> GetBudgetReportAsync(string token, int year = 0, int month = 0)
        {
            if (year < 2010)
            {
                year = DateTime.Now.Year;
            }

            var httpClient = ClientHelper.GetClient(token);
            {
                var serviceUrl = $"{Constants.ServiceAppUrl}/documents/deductibles/budget?client_token={token}&year={year}&month={month}";

                var response = await httpClient.GetAsync(serviceUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.GetContent<DeductibleLimitResponse>() ?? new DeductibleLimitResponse();

                    if (result.limits == null)
                    {
                        var limits = await GetDeduciblesAsync(token, year);

                        result.limits = limits.deductibles?.Select(x =>
                        {
                            var total = 0D;
                            double.TryParse(x.total, out total);
                            return new DeductibleLimit
                            {
                                id = x.id,
                                currentTotal = total,
                                maxValue = x.maxValue,
                                name = x.name
                            };
                        })?.ToList() ?? new List<DeductibleLimit>();
                    }

                    result.limits = result.limits.OrderBy(x => x.id).ToList();

                    return await Task.FromResult(result);
                }
            }

            return new DeductibleLimitResponse() { result = new Error() { code = "400", message = "Los datos ingresados son incorrectos" } };
        }


        public static async Task<OperationResult<saveDeductiblesLimitsResponse>> SavePresupuestoAsync(string token, List<DeductibleLimit> limits)
        {
            saveDeductiblesLimitsRequest budget = new saveDeductiblesLimitsRequest { client_token = token, deductibles = limits };

            var httpClient = ClientHelper.GetClient(token);
            {
                var serviceUrl = $"{Constants.ServiceAppUrl}/SaveDeductiblesLimits";
                var response = await httpClient.PostAsync(serviceUrl, budget.ToContent());

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.GetContentAsync<saveDeductiblesLimitsResponse>();

                    return new OperationResult<saveDeductiblesLimitsResponse>(true, System.Net.HttpStatusCode.OK, result) { UserMessage= "¡Se ha guardado el presupuesto!" };
                }
            }

            return new OperationResult<saveDeductiblesLimitsResponse>(false, System.Net.HttpStatusCode.InternalServerError) { UserMessage = "Hubo un error al guardar el presupuesto" };
        }


        public static async Task<DeductiblesReportResponse> GetDeduciblesAsync(string token, int year = 0)
        {
            if (year < 2010)
            {
                year = DateTime.Now.Year;
            }

            var httpClient = ClientHelper.GetClient(token);
            {
                var serviceUrl = $"{Constants.ServiceAppUrl}/documents/deductibles?year={year}";

                var response = await httpClient.GetAsync(serviceUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<DeductiblesReportResponse>(result);
                }
            }

            return new DeductiblesReportResponse() { result = new ApiResponse() { code = "400", message = "Los datos ingresados son incorrectos" } };
        }

    }
}
