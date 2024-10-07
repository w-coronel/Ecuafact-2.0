using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.Web.Models
{
    public class ErrorAPI
    {
        public ErrorAPI() { }

        public ErrorAPI(string errorID, string errorMessage)
        {
            this.ErrorID = errorID;
            this.ApiMessage = errorMessage;
        }

        public string AlertColor
        {
            get
            {
                return (ErrorID == "0") ? "alert-success" : "alert-warning";
            }
        }

        public string ApiMessage { get; set; }
        public string ErrorID { get; set; }
    }
}