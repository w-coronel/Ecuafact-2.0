using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Reporting;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Models;
using System;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace Ecuafact.Web.Controllers
{
    /// <summary>
    /// Este es un servicio externo desde EcuaFactApp
    /// </summary>
    [ExpressAuthorize]
    public class ReportesController : AppControllerBase
    {
        //private ServicioEmisor servicioEmisor = new ServicioEmisor();
        private const string OpenExcelFormat = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


        public ActionResult Index()
        {
            var model = new ObjectQueryModel(); 
            return View(model);
        }
           
        [HttpPost]
        public  ActionResult DescargarAsync(ObjectQueryModel model)
        {
            try
            {
                var response = ServicioReportes.ObtenerReporteAsync(IssuerToken, model.DocumentType, model.QueryType, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime())?.Result;

                var stream = response.Content.ReadAsStreamAsync()?.Result;

                return File(stream, response.Content.Headers.ContentType.ToString());
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Compras(ObjectQueryModel<PurchaseReportResponse> model)
        {
            var year = Convert.ToInt32(model.Year);
            var month = Convert.ToInt32(model.Month);
            var day = DateTime.DaysInMonth(year, month);
            var from = new DateTime(year, month, 1, 0, 0, 0, 0);
            var to = new DateTime(year, month, day, 23, 59, 59);
            var paramsDto = new ConsultaDocumentosParamsDto
            {
                FechaInicio = from,
                FechaHasta = to,
                Contenido = model.SearchTerm
            };

            var result = await ServicioReportes.ObtenerReporteCompras(IssuerToken, paramsDto.Contenido, paramsDto.FechaInicio, paramsDto.FechaHasta);

            if (result.IsSuccess)
            {
                model.Data = result;
            }

            Session["REPORTE_COMPRAS"] = model;

            return PartialView(model);
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Ventas(ObjectQueryModel<SalesReportResponse> model)
        {
            var year =Convert.ToInt32(model.Year) ;
            var month = Convert.ToInt32(model.Month);
            var day = DateTime.DaysInMonth(year, month);
            var from = new DateTime(year, month, 1, 0, 0, 0, 0);
            var to = new DateTime(year, month, day, 23, 59, 59);
            var paramsDto = new ConsultaDocumentosParamsDto
            {
                FechaInicio = from,
                FechaHasta = to,
                Contenido = model.SearchTerm
            };

            var result = await ServicioReportes.ObtenerReporteVentas(IssuerToken, paramsDto.Contenido, paramsDto.FechaInicio, paramsDto.FechaHasta);

            if (result.IsSuccess)
            {
                model.Data = result;
            }

            Session["REPORTE_VENTAS"] = model;

            return PartialView(model);
        }
         
        public enum ReportTypeEnum {
            Ventas,
            Compras,
            General
        }

        public ActionResult Excel(ReportTypeEnum id)
        {
            switch (id)
            {
                case ReportTypeEnum.Ventas:
                    return VentasToExcel();
                case ReportTypeEnum.Compras:
                    return ComprasToExcel();
                case ReportTypeEnum.General:
                    return GeneralToExcel();
            }

            return HttpNotFound("No se pudo generar la consulta!");
        }

        private ActionResult VentasToExcel()
        {
            var model = Session["REPORTE_VENTAS"] as ObjectQueryModel<SalesReportResponse>;

            if (model == null)
            {
                return HttpNotFound("No se encuentra la informacion del documento");
            }


            var filename = $"VENTAS_{model.From.Replace("/", "")}-{model.To.Replace("/", "")}";

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                filename += $"_{ model.SearchTerm.Replace("/", "").Replace(" ", "")}";
            }

            // Generamos el archivo excel
            var excelFile = SalesReport.GenerateExcel(model);


            return File(excelFile,  OpenExcelFormat, $"{filename}.xlsx");
        }

        private ActionResult GeneralToExcel()
        {
            throw new NotImplementedException();
        }

        private ActionResult ComprasToExcel()
        {
            var model = Session["REPORTE_COMPRAS"] as ObjectQueryModel<PurchaseReportResponse>;

            if (model == null)
            {
                return HttpNotFound("No se encuentra la informacion del documento");
            }
             
            var filename = $"COMPRAS_{model.From.Replace("/", "")}-{model.To.Replace("/", "")}";

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                filename += $"_{ model.SearchTerm.Replace("/", "").Replace(" ", "")}";
            }

            // Generamos el archivo excel
            var excelFile = PurchasesReport.GenerateExcel(model);

            return File(excelFile, OpenExcelFormat, $"{filename}.xlsx");
        }
    }

}
