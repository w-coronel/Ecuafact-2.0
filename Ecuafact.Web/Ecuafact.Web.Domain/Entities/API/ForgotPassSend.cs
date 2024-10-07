using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ecuafact.Web.Domain.Entities.API
{
    public class ForgotPassSend
    {
		[JsonProperty("id")]
		public string Id { get; set; }
    }

	public class ForgotPassResult : ErrorResponseModel { }
}
