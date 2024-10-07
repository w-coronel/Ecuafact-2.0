using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Ecuafact.Web.Controllers
{
    public abstract class AppControllerBase : Controller
    {
        protected virtual string SessionName => "EMPTY";

        protected string IssuerToken => SessionInfo.ApplicationToken;

        protected string SecurityToken => SessionInfo.SecurityToken;

        protected long IssuerId => SessionInfo.Issuer?.Id ?? 0;

        protected IssuerDto Issuer => SessionInfo.Issuer;

        protected HttpSessionState  Session => SessionInfo.Session;

        protected bool IsAuthenticated
        {
            get
            {
                return (User != null && User.Identity != null && User.Identity.IsAuthenticated);
            }
        }

        protected void ClearSession()
        {
            if (Session[SessionName] != null)
                Session[SessionName] = null;
            
            if (Session[SessionInfo.BORRADORES_SESSION] != null)
                Session[SessionInfo.BORRADORES_SESSION] = null;

            if (Session[SessionInfo.EMITIDOS_SESSION] != null)
                Session[SessionInfo.EMITIDOS_SESSION] = null;
        }

        // Establece los datos de lcas constantes para el emisor en el ViewBag
        protected override ViewResult View(IView view, object model)
        {
            ViewBag.IssuerId = this.IssuerId;
            ViewBag.Issuer = this.Issuer;

            return base.View(view, model);
        }
        
        protected System.Web.Http.HttpError GetValidationError()
        {
            var httpError = new System.Web.Http.HttpError("Error en la validacion de los datos:");

            var errors = this.ModelState.Where(x => x.Value.Errors.Count > 0).ToList();

            // SI HUBO UN ERROR DE VALIDACION:
            foreach (var item in errors)
            {
                var result = ((item.Value.Value != null) ? item.Value.Value.AttemptedValue : "null");

                var text = "";
                foreach (var error in item.Value.Errors)
                {
                    text += error.ErrorMessage;

                    if (error?.Exception != null)
                    {
                        text += ($"<br>  {error.Exception}");
                    }
                }

                httpError.Add(item.Key, $"{item.Key}: El valor '{result}' no es el correcto. {text}");
                httpError.Message += $"<br>- {text}";
            }
            
            return httpError;
        }


        protected static HttpCookieCollection RequestCookies
        {
            get { return System.Web.HttpContext.Current.Request.Cookies; }
        }

        protected static HttpCookieCollection ResponseCookies
        {
            get { return System.Web.HttpContext.Current.Response.Cookies; }
        }

        /// <summary>
        /// Determines whether the specified file exists
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>true if the caller has the required permissions and path contains the name of
        ///     an existing file; otherwise, false. This method also returns false if path is
        ///     null, an invalid path, or a zero-length string. If the caller does not have sufficient
        ///     permissions to read the specified file, no exception is thrown and the method
        ///     returns false regardless of the existence of path.</returns>
        protected bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

    }


    public abstract class DocumentControllerBase : AppControllerBase
    {
        protected abstract string DocumentType { get; }
        protected virtual DocumentosQueryModel QueryModel
        {
            get
            {
                var model = Session[SessionName] as DocumentosQueryModel;

                if (model == null)
                {
                    Session[SessionName] = model = new DocumentosQueryModel(DocumentType);
                }

                return model;
            }
            set { Session[SessionName] = value; }
        }

        public ActionResult Index()
        {
            // POR SEGURIDAD VERIFICA SI NO SE OBTUVO LA INFORMACION
            if (QueryModel == null)
            {
                QueryModel = new DocumentosQueryModel(DocumentType);
            }

            return View("Index", QueryModel);
        }


        //[System.Web.Mvc.HttpPost]
        //public async Task<ActionResult> GetDocumentsAsync(DocumentosQueryModel model)
        //{
        //    if (model == null)
        //    {
        //        throw new Exception("La solicitud especificada es no es válida!");
        //    }

        //    model.DocumentType = DocumentType;

        //    model.Data = await ServicioDocumento.BuscarDocumentosAsync(IssuerToken, model);

        //    if (model.Data == null)
        //    {
        //        model.Data = new List<DocumentModel>();
        //    }

        //    QueryModel = model;

        //    return PartialView("_DocumentosPartial", model);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="draw">Pagina</param>
        /// <param name="start">Inicio</param>
        /// <param name="length">Cantidad de registros x pagina</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetDocumentsAsync(DocumentosQueryModel model)
        {
            int draw = Convert.ToInt32(Request["draw"] ?? "1");
            int start = Convert.ToInt32(Request["start"] ?? "1");
            int length = Convert.ToInt32(Request["length"] ?? "10");
            string search = Request["search[value]"];
            long columna = Convert.ToInt32(Request["order[0][column]"]);
            string order = Request["order[0][dir]"];
            model.SearchTerm = model.SearchTerm ?? search;

            var page = 1;
            if (start > 0 && length > 0)
            {
                page = Convert.ToInt32(start / length) + 1;
            }

            var issuePCode = string.Empty;
            var estabCode = string.Empty;
            //validar si el emisor tiene rol de coperativa de transporte para obtener los documentos del establecimiento y punto de emision del usuario que tiene relacionado la cooperativa
            if (SessionInfo.UserRole == SecuritySessionRole.Cooperative)
            {
                SessionInfo.Issuer.Establishments.ForEach(s => {
                    var temp = s.IssuePoint.Where(d => d.CarrierRUC == SessionInfo.LoginInfo.UserInfo.Username)?.FirstOrDefault() ?? null;
                    if (temp != null)
                    {
                        issuePCode = temp.Code;
                        estabCode = SessionInfo.Issuer.Establishments.Where(e => e.Id == temp.EstablishmentsId)?.FirstOrDefault()?.Code;
                    }
                });
            }
            var document = await ServicioDocumento.ObtenerDocumentosPagedAsync(IssuerToken, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime(), page, length, documentType: model.DocumentType, status: model.Status, establishmentCode: estabCode, issuePointCode: issuePCode);
            var documentType = SessionInfo.Catalog.DocumentTypes.ToList();
            Session[SessionInfo.EMITIDOS_SESSION] = new DocumentosQueryModel() { Data = document.data };

            if (document.length > 0)
            {
                document.data.ToList().ForEach(d => {
                    d.DocumentName = documentType.Find(dt => dt.SriCode == d.DocumentTypeCode).Name;
                    d.Controller = Url.Action("Ver", Request.GetController(d.DocumentTypeCode), new { id = d.Id });
                    d.IconClass = Request.GetStatusIcon(d.Status.Value);
                    d.IconType = Request.GetDocumentIcon(d.DocumentTypeCode);
                    d.StyleColor = Request.GetColor(d.DocumentTypeCode);
                    d.StyleClass = d.Status.Value.GetValorCore() == "100" ? (d.AuthorizationDate == null ? DocumentStatusEnum.Authorized.GetStyle() : d.Status.Value.GetStyle()) : (d.Status.Value.GetStyle() ?? "brand");
                    d.StatusMessage = d.Status.Value.GetValorCore() == "100" ? (d.AuthorizationDate == null ? DocumentStatusEnum.Authorized.GetDisplayValue() : d.Status.Value.GetDisplayValue()) : (d.Status.Value.GetDisplayValue() ?? "Procesando");
                    d.StatusType = Request.GetStatusType(d.Status.Value);
                    d.StatusMsg = d.StatusMsg ?? $"El documento actual se encuentra en el estado: {d.StatusMessage}";
                    if (d.DocumentTypeCode == "06")
                    {
                        if (string.IsNullOrWhiteSpace(d.ContributorName) && string.IsNullOrWhiteSpace(d.ReferralGuideInfo.ReferenceDocumentAuth))
                        {
                            d.ContributorIdentification = d.ReferralGuideInfo.RecipientIdentification;
                            d.ContributorName = d.ReferralGuideInfo.RecipientName;
                        }
                    }
                    else if (d.DocumentTypeCode == "07")
                    {
                        if (d.RetentionInfo != null)
                        {
                            d.Total = d.RetentionInfo.FiscalAmount;
                        }
                    }
                });
            }

            // Solo es una validacion del request [es un ID request] 
            // debe ser el mismo que el requerido.
            document.draw = draw;
            return Json(document, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ver(string id)
        {
            var model = DocumentById(id);

            if (model == null)
            { 
                SessionInfo.Alert.SetAlert("No existe el documento especificado!", SessionInfo.AlertType.Error);
                return Index();
            }

            return View("Ver", model);
        }



        protected abstract object DocumentById(string id);

    }
}