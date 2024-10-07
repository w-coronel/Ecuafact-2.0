using Ecuafact.Web.Controllers;
using Ecuafact.Web.Domain.Entities.API;
using Ecuafact.Web.Domain.Services;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecuafact.Web.Areas.Carrier.Controllers
{
    public class UserController : AppControllerBase
    {
        // GET: Carrier/User
        public ActionResult Index()
        {
            return View();
        }

        // GET: Carrier/Perfil
        public ActionResult Perfil()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Perfil(ClientModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    var error = ModelState.Values?.FirstOrDefault(x => x.Errors.Count > 0)?.Errors?.FirstOrDefault();

                    throw error?.Exception ?? new Exception(error.ErrorMessage ?? "Los datos especificados no son correctos!");
                }

                if (model.Avatar != null && model.Avatar.ContentLength > 1048576)
                {
                    throw new Exception("El tamaño de la foto de perfil debe ser menor a 1MB");
                }

                var request = new profileUpdateRequest
                {
                    client_token = SecurityToken,
                    user_profile = new profile
                    {
                        businessName = model.Name,
                        name = model.Name,
                        email = model.Email,
                        phone = model.Phone,
                        streetAddress = model.Address
                    }
                };

                // Agregamos el nuevo usuario
                var response = ServicioUsuario.UpdateProfile(request);

                if (response?.UserInfo != null)
                {
                    SessionInfo.LoginInfo = response;

                    if (model.Avatar != null && model.Avatar.ContentLength > 0)
                    {
                        GuardarAvatar(model.Id, model.Avatar);
                    }

                    return Json(new OperationResult(true, System.Net.HttpStatusCode.OK, "Su perfil fue actualizado con éxito!"), JsonRequestBehavior.AllowGet);
                }

                throw new Exception("Hubo un error al guardar su perfil");
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(ex), JsonRequestBehavior.AllowGet);
            }
        }

        private void GuardarAvatar(string user, HttpPostedFileBase avatar)
        {
            try
            {
                var filename = Server.MapPath($"~/logos/{user}_logo.jpg");
                var bytes = avatar.GetBytes();

                System.IO.File.WriteAllBytes(filename, bytes);
            }
            catch { }
        }

        [HttpGet]
        public ActionResult Avatar(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = SessionInfo.UserInfo?.Username;
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
