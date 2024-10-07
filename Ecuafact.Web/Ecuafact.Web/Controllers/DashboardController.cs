using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorizeAttribute]
    public class DashboardController : AppControllerBase, IController
    {
        //
        // GET: /Dashboard/
        public async Task<ActionResult> Index()
        {
            if (SessionInfo.UserSession?.Subscription != null)
            {              
                var result = await ServicioEmisor.SubscriptionAsync(SessionInfo.UserSession.Token, SessionInfo.UserSession.Subscription.Id);
                if (result.IsSuccess)
                {
                    if (result.Entity.Status == SubscriptionStatusEnum.Activa)
                    {
                        SessionInfo.UserSession.Subscription.IssuedDocument = result.Entity.IssuedDocument;
                        SessionInfo.UserSession.Subscription.BalanceDocument = result.Entity.BalanceDocument;
                    }
                }
            }

            return View();
        }
        public async Task<ActionResult> LimiteDocuments()
        {
            var response = ServicioUsuario.ValidateToken(HttpContext.User.Identity.Name);
            if (response.UserInfo != null)
            {
                // Actualizamos la informacion actual del emisor
                AuthController.PerformIssuerAuthentication(response, true, SessionInfo.Issuer.RUC);
            }

            return View("Index");
        }

        public async Task<JsonResult> GetIssuedJsonAsync(string id)
        {
            var doc = await ServicioDocumento.BuscarDocumentosAsync(SessionInfo.ApplicationToken, "", DateTime.Now.AddDays(-30), DateTime.Now.AddHours(23).AddMinutes(59), documentType: id);

            var controller = (id == "01") ? "Factura"
                : (id == "07") ? "Retencion"
                : (id == "04") ? "NotaCredito"
                : (id == "06") ? "GuiaRemision"
                : (id == "03") ? "Liquidacion"
                : (id == "05") ? "NotaDeBito"
                : "Comprobantes";


            return Json(doc.Select(model => new
            {
                id = model.Id,
                typeDoc = model.DocumentTypeCode,
                docNumber = model.DocumentNumber,
                identificationNumber = model.ContributorIdentification,
                name = model.ContributorName,
                date = model.IssuedOn.ToString("dd/MM/yyyy"),
                total = model.Total.ToString("c", new CultureInfo("en-US")),
                authorizationNumber = model.AuthorizationNumber,
                authorizationDate = model.AuthorizationDate,
                month = model.IssuedOn.ToString("yyyy-MM"),
                url = Url.Action("Ver", controller, new { id = model.Id }),
                pdf = Url.Action("PDF", controller, new { id = model.Id })
            }), JsonRequestBehavior.AllowGet);

        }


        public async Task<JsonResult> GetReceivedJsonAsync(string id)
        {
            var parameters = new ConsultaDocumentosParamsDto
            {
                FechaInicio = DateTime.Now.AddDays(-30),
                FechaHasta = DateTime.Now.AddHours(23).AddMinutes(59),
                Contenido = "",
                TipoDocumento = id,
                TipoFecha = "1",
                NumeroPagina = 1,
                CantidadPorPagina = 10
            };

            var doc = await ServicioComprobantes.ObtenerComprobantesRecibidosAsync(SessionInfo.ApplicationToken, parameters);


            return Json(doc.documents.Select(model => new
            {
                id = model.pk,
                typeDoc = model.typeDoc,
                docNumber = model.sequence,
                identificationNumber = model.identificationNumber,
                name = model.name,
                date = model.date,
                total = model.total,
                authorizationNumber = model.authorizationNumber,
                authorizationDate = model.authorizationDate,
                month = model.date.Substring(3, 7),
                url = Url.Action("Ver", "Comprobantes", new { id = model.authorizationNumber }),
                pdf = Url.Action("Descargar", "Comprobantes", new { id = model.authorizationNumber, type = "PDF" })
            }), JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> GetIssuedFeedAsync(string id = null)
        {
            FeedModel feed = null;

            try
            {
                var doc = await ServicioDocumento.BuscarDocumentosAsync(SessionInfo.ApplicationToken, "", DateTime.Now.AddDays(-30), DateTime.Now.AddHours(23).AddMinutes(59));

                var list = doc
                    .OrderBy(m => new { date = m.LastModifiedOn ?? m.CreatedOn })
                    .Select(model => new FeedItemModel
                    {
                        Date = model.LastModifiedOn ?? model.CreatedOn,
                        Description = Request.GetDocumentType(model.DocumentTypeCode),
                        Icon = Request.GetDocumentIcon(model.DocumentTypeCode),
                        IconStyle = Request.GetStatusType(model.Status),
                        Url = Url.Action("ver", Request.GetController(model.DocumentTypeCode), new { model.Id })
                    });

                feed = new FeedModel(list);
            }
            catch (Exception ex)
            {
                feed = new FeedModel();
                SessionInfo.LogException(ex);
            }

            return PartialView("_Feed", feed);
        } 

    }
     
}
