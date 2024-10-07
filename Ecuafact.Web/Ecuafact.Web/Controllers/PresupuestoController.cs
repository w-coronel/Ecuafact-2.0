using Ecuafact.Web.Controllers;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using Ecuafact.Web.Filters;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ecuafact.Web.Domain.Services;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class PresupuestoController : AppControllerBase
    {
     
        // GET: Presupuesto
        public async Task<ActionResult> Index(int year = 0, int month = 0)
        {
            var result = await GetAsync(year, month);

            return View(result);
        }

        
        public async Task<ActionResult> GetBudgetDetailsAsync(int year = 0, int month = 0)
        {
            var model = await GetAsync(year, month);

            return PartialView("_DetalleDeducibles", model);
        }

        public async Task<ActionResult> GetReporteDeduciblesAsync(int year = 0, int month = 0)
        {
            DeductiblesReportResponse result = null;

            try
            {
                if (year < 2010)
                {
                    year = DateTime.Now.Year;
                }

                result = await GetAsync(year, month);
            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);

                result = new DeductiblesReportResponse
                {
                    deductibles = new List<DeductibleSum>(),
                    result = new ApiResponse { code = "404", message = ex.Message }
                };
            }

            return PartialView("_ReporteDeducibles", result);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarAsync(DeductibleLimitResponse model)
        {
            try
            {
                var result = await ServicioGastos.SavePresupuestoAsync(IssuerToken, model.limits);

                if (result.IsSuccess)
                {
                    SessionInfo.Presupuesto = await ServicioGastos.GetBudgetReportAsync(IssuerToken, DateTime.Now.Year);
                } 

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.InternalServerError, ex.Message), JsonRequestBehavior.AllowGet);
            }
            
        }


        private static async Task<DeductiblesReportResponse> GetAsync(int year, int month)
        {
            DeductiblesReportResponse result = null;

            try
            {
                if (year < 2010)
                {
                    year = DateTime.Now.Year;
                }
                if (month < 1)
                {
                    month = 0;
                }
                else if (month > 12)
                {
                    month = 12;
                }

                var budget = await ServicioGastos.GetBudgetReportAsync(SessionInfo.ApplicationToken, year, month);

                result = new DeductiblesReportResponse
                {
                    deductibles = budget.limits?.Select(x => new DeductibleSum
                    {
                        id = x.id,
                        name = x.name,
                        maxValue = x.maxValue,
                        total = x.currentTotal.ToString()?? "0.00"
                    }).ToList()?? new List<DeductibleSum>(),
                    result = new ApiResponse { code = "200", message = "OK" }
                };
            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);

                result = new DeductiblesReportResponse
                {
                    deductibles = SessionInfo.Presupuesto.limits?.Select(x => new DeductibleSum
                    {
                        id = x.id,
                        maxValue = x.maxValue,
                        name = x.name,
                        total = x.currentTotal.ToString() ?? "0.00"
                    }).ToList() ?? new List<DeductibleSum>(),
                    result = new ApiResponse { code = "404", message = ex.Message }
                };
            }

            return result;
        }

    }
}