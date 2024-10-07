using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Ecuafact.WebAPI.Integration.Controllers
{
    public class ApiControllerBase : ApiController, IHttpController, IDisposable
    {



        protected string GetClientToken(string id)
        {
            // Primero buscamos al subscriptor en la base del Vendor

            var user = User as ClaimsPrincipal;
            if (user!= null)
            {
                var vendorToken = user.Claims.FirstOrDefault(m => m.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid");
                
            }



            return "636b4be1-6095-429e-af17-71195d2a7f74";
        }


        public Vendor Vendor => GetVendor();

        private Vendor GetVendor()
        {
            return null;
        }
    }
}