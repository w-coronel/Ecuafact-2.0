using Ecuafact.Web.Controllers;
using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Ecuafact.Web.Domain.Services;
using System.Security.Cryptography;
using System.Net.Http;
using Ecuafact.Web.Models;
using System.Security.Cryptography.X509Certificates;
using Ecuafact.Web.Domain.Extensions;
using System.Net;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class EmisorController : AppControllerBase
    {
        public string ReturnUrl
        {
            get { return Session["EmisorController.ReturnUrl"] as string; }
            set { Session["EmisorController.ReturnUrl"] = value; }
        }

        // GET: Emisor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActualizarAsync(string ruc = null)
        {
            var model = GetIssuerModel(ruc);
            return PartialView("Configuracion", model);
        }

        [HttpGet]
        public async Task<ActionResult> Configuracion(string ruc = null)
        {
            var model = GetIssuerModel(ruc);
            if (model == null)
            {
                return RedirectToAction("Nuevo", new { ruc });
            }
            var _issuePoint = SessionInfo.UserSession?.Subscription?.AmountIssuePoint ?? 0;
            if (_issuePoint > 0)
            {
                var issuePoints = await ServicioEmisor.GetIssuePointsByIssuer(IssuerToken);
                model.AmountIssuePoints = issuePoints?.Count ?? 0;
            }

            return await Task.FromResult(View(model));
        }

        [HttpGet]
        public JsonResult ValidarEmisor(string ruc = null)
        {
            IssuerDto model = null;

            if (!string.IsNullOrEmpty(ruc))
            {
                model = ServicioEmisor.ObtenerEmisorPorRUC(SecurityToken, ruc);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Nuevo(string ruc = null)
        {
            var model = new SRIRequestModel();

            if (ruc == null && SessionInfo.Issuer == null)
            {
                ruc = (SessionInfo.LoginInfo?.UserInfo?.Username.Length == 10
                                  ? SessionInfo.LoginInfo?.UserInfo.Username.Trim() + "001" // Si es cedula
                                  : SessionInfo.LoginInfo?.UserInfo.Username);
            }

            model.RUC = ruc ?? "";
            
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Nuevo(SRIRequestModel model)
        {
            try
            {
                var contrib = await ServicioEmisor.CrearAsync(SecurityToken, model);
                if (contrib.IsSuccess)
                {
                    // verificamos si el emisor existe:
                    return View("Configuracion", contrib.Entity);
                }

                throw new Exception(contrib?.UserMessage);
            }
            catch (Exception ex)
            {
                SessionInfo.Alert.SetAlert(ex.Message, SessionInfo.AlertType.Error);
                return View(new SRIRequestModel { RUC = model.RUC });
            } 
        }

        [HttpPost]
        public async Task<JsonResult> Configuracion(IssuerDto model)
        {
            var errors = "";
            try
            {
                var _url = ReturnUrl; 
                if (model == null)
                {
                    throw new Exception("Los datos enviados son inválidos!");
                }

                // Control para los casos de los emisores que por error no adjuntan sus certificados
                if (model.CertificateRaw == null && !string.IsNullOrEmpty(model.CertificatePass))
                {
                    // Si el certificado que fue validado en cache fue invalido no es tomado en cuenta.
                    if (Session[model.CertificatePass] == null || model.CertificateExpirationDate == null)
                    {
                        // Dejamos la información de los certificados como en el original:
                        model.Certificate = null;
                        model.CertificatePass = null;
                        model.CertificateExpirationDate = null;
                        model.CertificateIssuedTo = null;
                        model.CertificateSubject = null;
                        model.CertificateRUC = null;
                        model.CertificateUsage = null;
                    }
                    else
                    {
                        model.CertificateRaw = Session[model.CertificatePass] as HttpPostedFileBase;
                    }
                }

                if (model.LogoFile == null)
                {
                    // Generamos un logo por defecto, si el usuario no sube su propio logo
                    if (!Server.ExistsLogoFile(model.RUC))
                    {
                        model.Logo = $"{model.RUC}_Logo.jpg";
                        model.LogoFile = Avatar.Generate(model.BussinesName.ToUpper().FirstOrDefault().ToString());
                    }                  
                   
                }
               
                var result = await ServicioEmisor.GuardarAsync(SecurityToken, model);

                if (result.IsSuccess)
                {
                    var response = ServicioUsuario.ValidateToken(HttpContext.User.Identity.Name);

                    if (response.UserInfo != null)
                    {
                        // Actualizamos la informacion actual del emisor
                        AuthController.PerformIssuerAuthentication(response, true, model.RUC);
                    }

                    //SessionInfo.Alert.SetAlert("¡Se ha guardado el emisor!", SessionInfo.AlertType.Success);
                     
                    var buyCertificate = Request.Form["BuyCertificate"] == "1";

                    if (buyCertificate)
                    {
                        var pendingResult = await ServicioFirma.BuscarPendienteAsync(IssuerToken, model.RUC);

                        if (pendingResult.IsSuccess && pendingResult.Entity != null)
                        {
                            _url = Url.Action("Ver", "FirmaElectronica", new { id = pendingResult.Entity.Id });
                            //return RedirectToAction("Ver", "FirmaElectronica", new { id = pendingResult.Entity.Id });
                        }
                        else
                        {
                            _url = Url.Action("Nuevo", "FirmaElectronica");
                            //return RedirectToAction("Nuevo", "FirmaElectronica");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(ReturnUrl))
                        {
                            _url = ReturnUrl?.Replace("Nuevo", "Configuracion");
                            //return Redirect(ReturnUrl?.Replace("Nuevo", "Configuracion") ?? "");
                        }
                        else
                        {
                            _url = Url.Action("Index");
                        }
                    }
                    
                    //return RedirectToAction("Index");

                    return new JsonResult
                    {
                        Data = new
                        {
                            id = result.Entity.Id,
                            result = result.Entity,
                            error = default(object),
                            status = SessionInfo.AlertType.Success,
                            statusText = "¡Se ha guardado el emisor!",
                            url = _url
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }
                else
                {
                    errors = result.UserMessage ?? $"Hubo un error al guardar el emisor. {SessionInfo.AlertType.Error}";
                    //SessionInfo.Alert.SetAlert(result.UserMessage ?? "Hubo un error al guardar el emisor.", SessionInfo.AlertType.Error);
                }

            }
            catch (Exception ex)
            {
                errors = $"Hubo un error al guardar el emisor: {ex.Message},  {SessionInfo.AlertType.Error}";
                //SessionInfo.Alert.SetAlert($"Hubo un error al guardar el emisor: {ex.Message}", SessionInfo.AlertType.Error);
            }

            return new JsonResult()
            {
                Data = new
                {
                    id = 0,
                    result = default(object),
                    error = errors,
                    status = HttpStatusCode.InternalServerError,
                    statusText = $"Error en el emisor. {errors}"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };
        }

        [HttpPost]
        public JsonResult ValidateCertificate(CertificateInfo certificate)
        {
            try
            {
                if (certificate == null || certificate.CertificateRaw == null)
                {
                    throw new Exception("Debe seleccionar un certificado de firma electrónica válido.");
                }
                
                Session[certificate.CertificatePass] = certificate.CertificateRaw;
                var cert = ServicioEmisor.ValidateCertificate(certificate.CertificateRaw, certificate.CertificatePass);                
                var CertificateRUC = cert.GetExtensionString("1.3.6.1.4.1.%.3.11");
                var usage = cert.GetExtension<X509KeyUsageExtension>();
                
                var result = new OperationResult<object>(true, System.Net.HttpStatusCode.OK,
                new
                {
                    cert.FriendlyName,
                    cert.Issuer,
                    cert.IssuerName,
                    ExpiryDate= cert.NotAfter,
                    ValidDate = cert.NotBefore,
                    cert.SerialNumber,
                    cert.Version,
                    cert.Subject,
                    cert.Thumbprint,
                    cert.SubjectName,
                    ExpirationDate = cert.GetExpirationDateString(),
                    CertificateRUC,
                    KeyUsages = usage?.KeyUsages.ToString()
                });

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (CryptographicException ex)
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest)
                {
                    DevMessage = ex.ToString(),
                    UserMessage = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest)
                {
                    DevMessage = ex.ToString(),
                    UserMessage = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        private IssuerDto GetIssuerModel(string ruc)
        {
            ReturnUrl = Request.UrlReferrer?.ToString();
             
            if (string.IsNullOrEmpty(ruc))
            {
                ruc = SessionInfo.Issuer?.RUC ??
                        (SessionInfo.LoginInfo?.UserInfo?.Username.Length == 10
                                   ? SessionInfo.LoginInfo?.UserInfo.Username.Trim() + "001" // Si es cedula
                                   : SessionInfo.LoginInfo?.UserInfo.Username);
            }

            return ServicioEmisor.ObtenerEmisorPorRUC(SecurityToken, ruc);
        }


        [HttpGet]
        public ActionResult Logo(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Issuer?.RUC;
            }

            var logo = Server.MapPath(Server.GetLogoFile(id));

            if (FileExists(logo))
            {
                var img = System.IO.File.ReadAllBytes(logo);
                return File(img, "image/jpg");
            }

            return HttpNotFound();
        }
    }
}