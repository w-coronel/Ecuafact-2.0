using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ecuafact.Web.Domain.Entities.API
{
	public class ErrorResponseModel
	{
		[JsonProperty("result")]
		public ErrorModel Result { get; set; }

		[JsonProperty("error")]
		public ErrorModel Error { get; set; }
	}
      
	public class ErrorModel
	{
		[JsonProperty("code")]
		public string Code { get; set; }
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}