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

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class GastosController : AppControllerBase
    {
        public DeductiblesReportResponse Deducibles {
            get
            {
                return Session["Deducibles"] as DeductiblesReportResponse;
            }
            set
            {
                Session["Deducibles"] = value;
            }
        }

        // GET: Gastos
        public async Task<ActionResult> Index(int year = 0, int month = 0)
        {
            var result = await GetAsync(year, month);

            return View(result);
        }

        public async Task<DeductiblesReportResponse> GetAsync(int year, int month)
        {
            DeductiblesReportResponse result = null;

            try
            {
                if (year < 2010)
                {
                    year = DateTime.Now.Year;
                }

                result = await ServicioGastos.GetExpenseReportAsync(IssuerToken, year, month);
            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);

                result = new DeductiblesReportResponse
                {
                    deductibles = Deducibles.deductibles,
                    result = new ApiResponse { code = "404", message = ex.Message }
                };
            }


            return result;
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

                result = await ServicioGastos.GetExpenseReportAsync(IssuerToken, year, month);
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
    }
}