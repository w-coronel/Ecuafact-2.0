using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ecuafact.Web.Controllers
{
    public class AtsController : AppControllerBase
    {
        
        public async Task<ActionResult> Index()
        {

            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                if(SessionInfo.UserSession?.Subscription.LicenceType.Code =="L03" || SessionInfo.UserSession?.Subscription.LicenceType.Code == "L04")
                {
                    var model = new ObjectQueryModel<AtsFacturaReportResponse>();
                    var _date = DateTime.Now;
                    var result = await ServicioReportes.ObtenerReporteFacturas(IssuerToken, PeriodTypeEnum.Monthly, _date.Year, _date.Month, 0);
                    if (result.IsSuccess)
                    {
                        SessionInfo.StatisticsAts = await ServicioReportes.ObtenerEstadisticasComprasAts(IssuerToken, PeriodTypeEnum.Monthly, _date.Year, _date.Month, 0);
                        model.Data = result;
                    }
                    return View(model);
                }
                else
                {
                    SessionInfo.Alert.SetAlert("Esta funcionalidad no esta disponible en su licencia actual", SessionInfo.AlertType.Info);
                    return RedirectToAction("Index", "Dashboard");
                }
               
            }
            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AtsCompras(ObjectQueryModel<AtsFacturaReportResponse> model)
        {
            var periodType = PeriodTypeEnum.Monthly;
            if(model.Period == 2)
            {
                periodType = PeriodTypeEnum.Biyearly;
            }           
            var year = Convert.ToInt32(model.Year);
            var month = Convert.ToInt32(model.Month);
            var semester = model.Period;           
            var result = await ServicioReportes.ObtenerReporteFacturas(IssuerToken, periodType, year, month, semester);
            if (result.IsSuccess)
            {
                SessionInfo.StatisticsAts = await ServicioReportes.ObtenerEstadisticasComprasAts(IssuerToken, periodType, year, month, semester);               
                model.Data = result;                
            }
            Session["REPORTE_ATS_COMPRAS"] = model;
            return PartialView(model);

        }

        [HttpGet]
        public async Task<ActionResult> DescargarAtsAsync(string format, PeriodTypeEnum periodType, int year, int month, int semester)
        {            
            var result = await ServicioReportes.DescargarAtsAsync(IssuerToken, format, periodType, year, month, semester);
            var content = await result.Content.ReadAsStreamAsync();
            return File(content, result.Content.Headers.ContentType.ToString(), result.Content.Headers.ContentDisposition.FileName);
        }

        [HttpGet]
        public async Task<ActionResult> TipoSustento(int id, int emissionType)
        {
            var document = await ServicioReportes.ObtenerComprasAtsById(IssuerToken, id, emissionType);
            return PartialView("_TipoSustento", document);
        }

        [HttpPost]
        public async Task<ActionResult> Sustento(long id, string supTypeCode, int emissionType)
        {
            var response = await ServicioComprobantes.TipoSustentoDocumento(IssuerToken, id, supTypeCode, emissionType).ConfigureAwait(false);
            return Json(new { response }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> StatisticsAtsAsync(PeriodTypeEnum periodType, int year, int month, int semester)
        {
            var result = await ServicioReportes.ObtenerEstadisticasComprasAts(IssuerToken, periodType, year, month, semester);
            if(result != null)
            {
                SessionInfo.StatisticsAts = result;
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            SessionInfo.StatisticsAts = new StatisticsAts();
            return new JsonResult { Data = new StatisticsAts(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult StatisticsLocalAtsAsync()
        {
            var data = SessionInfo.StatisticsAts;
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}