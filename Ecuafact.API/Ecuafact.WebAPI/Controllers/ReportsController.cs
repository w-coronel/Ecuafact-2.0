
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Ecuafact.WebAPI.Domain.Reporting;
using Ecuafact.WebAPI.Domain.Extensions;
using Ecuafact.WebAPI.Models;
using System.ComponentModel;
using System.Text;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// REPORTES: Servicio de Informacion de los documentos electronicos
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Reportes")]
    public class ReportsController : ApiController
    {
        private static IIssuersService _issuersService;
        private static IReportService _reportService;
        private readonly IStatisticsService _statisticsService;
        private readonly IAtsService _atsService;

        public IssuerDto Issuer => this.GetAuthenticatedIssuer();
        public ReportsController(IIssuersService issuersService, IReportService reportService, IStatisticsService statisticsService, IAtsService atsService)
        {
            _issuersService = issuersService;
            _reportService = reportService;
            _statisticsService = statisticsService;
            _atsService = atsService;
        }

        /// <summary>
        /// Consultar Ventas
        /// </summary>
        /// <remarks>
        ///     Permite consultar las facturas de ventas con sus respectivas retenciones
        /// </remarks> 
        /// <param name="startDate">Fecha de Inicio</param>
        /// <param name="endDate">Fecha de Finalizacion</param>
        /// <param name="search">Termino de Busqueda</param>
        /// <returns></returns>
        [HttpGet, Route("reports/sales")]
        public OperationResult<IEnumerable<SalesReport>> GetSalesReport(string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            { 
                var report = _reportService.GetSalesReport(Issuer.RUC, startDate, endDate, search);
                return new OperationResult<IEnumerable<SalesReport>>(true, System.Net.HttpStatusCode.OK, report);
            }
            catch (Exception ex)
            {
                return new OperationResult<IEnumerable<SalesReport>>(false, System.Net.HttpStatusCode.InternalServerError, new List<SalesReport>())
                {
                    DevMessage = ex.ToString(),
                    UserMessage = "Error al generar el reporte de ventas!"
                };
            }
            
        }

        /// <summary>
        /// Consultar Compras
        /// </summary>
        /// <remarks>
        ///     Permite consultar las facturas de compras con sus respectivas retenciones
        /// </remarks> 
        /// <param name="startDate">Fecha de Inicio</param>
        /// <param name="endDate">Fecha de Finalizacion</param>
        /// <param name="search">Termino de Busqueda</param>
        /// <returns></returns>
        [HttpGet, Route("reports/purchases")]
        public OperationResult<IEnumerable<PurchaseReport>> GetPurchasesReport(string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            { 
                var report = _reportService.GetPurchaseReport(Issuer.RUC, startDate, endDate, search);
                return new OperationResult<IEnumerable<PurchaseReport>>(true, System.Net.HttpStatusCode.OK, report);
            }
            catch (Exception ex)
            {
                return new OperationResult<IEnumerable<PurchaseReport>>(false, System.Net.HttpStatusCode.InternalServerError, new List<PurchaseReport>())
                {
                    DevMessage = ex.ToString(),
                    UserMessage = "Error al generar el reporte de ventas!"
                };
            }

        }


        /// <summary>
        /// Descargar Ventas
        /// </summary>
        /// <remarks>
        /// Permite descargar el Reporte de Ventas en varios formatos
        /// </remarks>
        /// <param name="search">Texto de Busqueda</param>
        /// <param name="startDate">Fecha de Inicio</param>
        /// <param name="endDate">Fecha de Inicio</param>
        /// <param name="format">Formato del Documento a descargar: PDF, Excel, Word</param>
        /// <returns></returns>
        [HttpGet, Route("reports/{format}/sales")]
        public HttpResponseMessage SalesReportDownload(string format = "Excel", string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {  
            var data = _reportService.GetSalesReport(Issuer.RUC, startDate, endDate, search);

            var salesReport = new ReportSales(search, startDate, endDate);
            var report = salesReport.Render(data, format);
            
            // Ahora generamos el archivo segun el tipo especificado:
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(report.Content);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(report.MimeType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = $"Ventas_{startDate?.ToString("yyyy-MM-dd")}_{endDate?.ToString("yyyy-MM-dd")}.{report.Extension}"
            };

            return result;
        }


        /// <summary>
        /// Descargar Compras
        /// </summary>
        /// <remarks>
        /// Permite descargar el Reporte de Compras en varios formatos
        /// </remarks>
        /// <param name="format">Formato del Documento a descargar: PDF, Excel, Word</param>
        /// <param name="search">Texto de Busqueda</param>
        /// <param name="startDate">Fecha de Inicio</param>
        /// <param name="endDate">Fecha de Fin</param>
        /// <returns></returns>
        [HttpGet, Route("reports/{format}/purchases")]
        public HttpResponseMessage PurchaseReportDownload(string format = "Excel", string search = null, DateTime? startDate = null, DateTime? endDate = null )
        { 
            var data = _reportService.GetPurchaseReport(Issuer.RUC, startDate, endDate, search);

            var purchaseReport = new ReportPurchases(search, startDate, endDate); 
            var report = purchaseReport.Render(data, format);

            // Ahora generamos el archivo segun el tipo especificado:

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(report.Content);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(report.MimeType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = $"Compras_{startDate?.ToString("yyyy-MM-dd")}_{endDate?.ToString("yyyy-MM-dd")}.{report.Extension}"
            };

            return result;
        }

        /// <summary>
        /// Listar facturas, nota de venta y liquidacion
        /// </summary>
        /// <remarks>
        /// Lista las facturas, nota de venta y liquidaciones
        /// </remarks>        
        /// <param name="periodType">Tipo de periodo: Anual o Semestral</param>
        /// <param name="year">El numero del año</param>
        /// <param name="month">El numero de mes/param>
        /// <param name="semester">El numero semestre 1 0 2</param>
        /// <returns></returns>
        [HttpGet, Route("reports/ats/purchases")]
        public OperationResult<IEnumerable<AtsFactura>> AtsReportPurchases(PeriodTypeEnum? periodType = null, int year = 0, int month = 0, int semester = 0)
        {
            try
            {
                var report = _atsService.GetReportInvoiceAts(Issuer.RUC, periodType.Value, year, month, semester);
                return new OperationResult<IEnumerable<AtsFactura>>(true, System.Net.HttpStatusCode.OK, report.Entity.ToAtsFactura());
            }
            catch (Exception ex)
            {
                return new OperationResult<IEnumerable<AtsFactura>>(false, System.Net.HttpStatusCode.InternalServerError, new List<AtsFactura>())
                {
                    DevMessage = ex.ToString(),
                    UserMessage = "Error al generar el reporte de facturas!"
                };
            }
        }


        /// <summary>
        /// Listar facturas, nota de venta y liquidacion
        /// </summary>
        /// <remarks>
        /// Lista las facturas, nota de venta y liquidaciones
        /// </remarks>
        /// <param name="id">identificador del documento</param>
        /// <param name="typeIssuance">Tipo de emision recibida o emitida</param>        
        
        /// <returns></returns>
        [HttpGet, Route("reports/ats/{id}/purchase")]
        public async Task<HttpResponseMessage> AtsPurchasesById(int id, int typeIssuance = 1)
        {
            try
            {
                var report = await Task.FromResult(_atsService.GetPurchasesAtsById(id, typeIssuance));
                if (report.IsSuccess)
                {
                    return Request.CreateResponse<AtsFactura>(HttpStatusCode.OK, report.Entity.ToAtsFactura());
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }
        }

        /// <summary>
        /// Descargar Ats
        /// </summary>
        /// <remarks>
        /// Permite descargar el Reporte de Ats en excel y xml
        /// </remarks>
        /// <param name="format">Formato del Documento a descargar: PDF, XML</param>
        /// <param name="periodType">Tipo de periodo: Anual o Semestral</param>
        /// <param name="year">El numero del año</param>
        /// <param name="month">El numero de mes/param>
        /// <param name="semester">El numero semestre 1 0 2</param>
        /// <returns></returns>
        [HttpGet, Route("reports/{format}/ats")]
        public HttpResponseMessage AtsReportDownload(string format = "excel", PeriodTypeEnum? periodType = null, int year = 0, int month = 0, int semester = 0)
        {
            try
            {
                var _response = _atsService.GetReportAts(Issuer.RUC, periodType.Value, year, month, semester);
                var period = periodType == PeriodTypeEnum.Monthly ? month.MonthName() : (semester == 1 ? "Enenero-Junio" : "Julio-Diciembre");
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                if (format.Equals("xml"))
                {
                    var _xml = _response.Entity.ToAtsXml();
                    if (_xml != null)
                    {
                        var report = new ReportResult(_xml)
                        {
                            Encoding = $"{Encoding.UTF8}",
                            MimeType = "text/xml",
                            Extension = "xml"
                        };
                        Logger.Log($"LOG.{Issuer.RUC}.ATS.XML.CREATE.{DateTime.Now.ToFileTime()}", "Request Message: ", _response.Entity);
                        var _stream = new MemoryStream(report.Content);
                        result.Content = new StreamContent(_stream);
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue(report.MimeType);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"Ats_{year}_{period}_{Issuer.RUC}.{report.Extension}"
                        };
                        return result;
                    }
                }
                else
                {
                    var excel = _response.Entity.ToAtsReports();
                    if (excel != null)
                    {
                        var stream = new MemoryStream(excel);
                        result.Content = new StreamContent(stream);
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/excel");
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"Ats_{year}_{period}_{Issuer.RUC}.xlsx"
                        };
                        return result;
                    }
                }

                return Request.BuildHttpErrorResponse(HttpStatusCode.NoContent, $"Error al generar el reporte Ats");
            }
            catch (HttpResponseException ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.ATS.XML.ERROR.{DateTime.Now.ToFileTime()}", "Request Message: ", ex.Response);
                return ex.Response;
            }
            catch (Exception ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.ATS.XML.ERROR.{DateTime.Now.ToFileTime()}", "Request Message: ", $"{ex.Message} {ex.InnerException?.Message}");
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}", $"Hubo un error al procesar el Ats: {ex.Message}");
            }
        }


        /// <summary>
        /// cantidad de docuemntos recibidos para generar el ats
        /// </summary>
        /// <remarks>
        /// Devuelve la información estadística de los docuemntos recibidos
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("reports/statistics/ats")]
        public OperationResult<StatisticsAts> GetStatisticsAts(PeriodTypeEnum? periodType = null, int year = 0, int month = 0, int semester = 0)
        {
            try
            {
                if (year == 0)
                    year = DateTime.Now.Year;
                if (periodType == null)
                    periodType = PeriodTypeEnum.Monthly;
                if(month== 0)
                    month = DateTime.Now.Month;

                var report = _atsService.GetStatisticsAts(Issuer.RUC, periodType.Value, year, month, semester);
                return report;
            }
            catch (Exception ex)
            {
                return new OperationResult<StatisticsAts>(false, System.Net.HttpStatusCode.InternalServerError)
                {
                    DevMessage = ex.ToString(),
                    UserMessage = "Error al generar el reporte de facturas!"
                };
            }
        }

        /// <summary>
        /// Estadísticas Generales
        /// </summary>
        /// <remarks>
        /// Devuelve la información estadística de la aplicación
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("statistics")]
        public DashboardInfo GetDashboard()
        {
            var issuer = this.GetAuthenticatedIssuer();
            var model = _statisticsService.GetDashboard(issuer.Id);

            return model;
        }
    }
}
