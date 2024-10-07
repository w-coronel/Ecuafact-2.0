using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ecuafact.Web.Domain.Entities.API
{
	public class SriSend
	{
		[JsonProperty("login")]
		public SriLogin Login { get; set; }
	}
	public class SriResult : ErrorResponseModel
	{
		[JsonProperty("client")]
		public SriClient Client { get; set; }
	}
	public class SriLogin
	{
		[JsonProperty("token")]
		public string Token { get; set; }
		[JsonProperty("user")]
		public string User { get; set; }
		[JsonProperty("password")]
		public string Password { get; set; }
	}

	public class SriLoginResult
	{
		[JsonProperty("contenido")]
		public string Contenido { get; set; }
		[JsonProperty("mensaje")]
		public string Mensaje { get; set; }
		[JsonProperty("codigo")]
		public string Codigo { get; set; }
		[JsonProperty("detalles")]
		public string Detalles { get; set; }

	}

	public class SriClient
	{
		[JsonProperty("businessName")]
		public string BusinessName { get; set; }
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("email")]
		public string Email { get; set; }
		[JsonProperty("message")]
		public string Message { get; set; }
		[JsonProperty("streetAddress")]
		public string StreetAddress { get; set; }
		[JsonProperty("clientType")]
		public string ClientType { get; set; }
		[JsonProperty("clientToken")]
		public string ClientToken { get; set; }
	}
}
