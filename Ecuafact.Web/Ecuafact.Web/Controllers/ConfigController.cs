using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Services;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using System.Net;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class ConfigController : AppControllerBase
    {
        // GET: Config
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var issuer = SessionInfo.Issuer ?? new IssuerDto();

            // Si no existe el emisor entonces cargamos los datos predeterminados.
            if (SessionInfo.Issuer == null)
            {
                if (SessionInfo.LoginInfo != null)
                {
                    var userInfo = SessionInfo.LoginInfo?.UserInfo;

                    issuer.BussinesName = issuer.TradeName = userInfo.Name;
                    issuer.RUC = (userInfo.Username.Length == 10)
                            ? userInfo.Username.Trim() + "001" // Si es cedula
                            : userInfo.Username;
                    issuer.MainAddress = userInfo.Address;
                    issuer.Email = userInfo.Email;
                }

                issuer.CertificatePass = "";
                issuer.EnvironmentType = EnvironmentTypeEnum.Producción;
                issuer.EstablishmentCode = "001";
                issuer.IssuePointCode = "001";
                issuer.IssueType = IssueTypeEnum.Normal;
                issuer.IsAccountingRequired = false;
                issuer.IsSpecialContributor = false;
                issuer.ResolutionNumber = "";
                issuer.IsEnabled = true;
            }

            var esign = await ServicioFirma.BuscarAsync(this.IssuerToken, null, null);
            issuer.viewRequests = (esign?.Count > 0);

            var logoFile = GetDefaultLogo(issuer);

            ViewData["LogoFile"] = logoFile;

            var budget = await ServicioGastos.GetBudgetReportAsync(this.IssuerToken, DateTime.Today.Year, 1);

            var model = new ConfigModel
            {
                UserProfile = SessionInfo.UserInfo,
                Issuer = issuer,
                Budget = budget
            };

            return View(model); 
        }
         
        [HttpPost]
        public async Task<ActionResult> ConfigurarAsync(ConfigModel model)
        {
            if (model.Issuer != null)
            {

                var result = await ServicioEmisor.GuardarAsync(SecurityToken, model.Issuer);


                if (result.IsSuccess)
                {
                    var response = ServicioUsuario.ValidateToken(HttpContext.User.Identity.Name);

                    if (response.UserInfo != null)
                    {
                        // Actualizamos la informacion actual del emisor
                        AuthController.PerformIssuerAuthentication(response, true, model.Issuer.RUC);
                    }
                    
                    SessionInfo.Notifications.Add("¡Se ha guardado el emisor!", SessionInfo.AlertType.Success);
                }
                else
                {
                    SessionInfo.Notifications.Add(result.UserMessage ?? "Hubo un error al guardar el emisor.", SessionInfo.AlertType.Error);
                }
            }

            if (model?.Budget?.limits?.Count > 0)
            {
                var result = await ServicioGastos.SavePresupuestoAsync(SecurityToken, model.Budget.limits);
                if (result.IsSuccess)
                {
                    SessionInfo.Notifications.Add("¡Se ha guardado el emisor!", SessionInfo.AlertType.Success);
                }
                else
                {
                    SessionInfo.Notifications.Add(result.UserMessage ?? "Hubo un error al guardar el emisor.", SessionInfo.AlertType.Error);
                }
            }


            return RedirectToAction("", "Auth");
        }

        [HttpPost]
        public async Task<JsonResult> Desvincular(string id)
        {            
            var result = await ServicioUsuario.DesvincularCuenta(SecurityToken, SessionInfo.LoginInfo.UserInfo.Username, id);
            if (result.IsSuccess)
            {
                SessionInfo.LoginInfo.Issuers = result.Entity;

                return Json(new { id = 1, message = $"La desvinculación del ruc {id} fue exitosa!" ,JsonRequestBehavior.AllowGet });

            }
            return Json(new { id = 0, message = $"Error al desvincular el ruc {id}: " + result.UserMessage, JsonRequestBehavior.AllowGet });
        }

        private string GetDefaultLogo(IssuerDto model)
        {
            return Url.Content(Server.GetLogoUrl(model.Logo, model.RUC));
        }
    }
}