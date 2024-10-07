using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace System
{
    public class JsonWebApiHelper
    {
		public static T ExecuteJsonWebApi<T>(string url, HttpMethod httpMethod, object request = null)
		{
			// Ejecutamos la operación WEB
			string jsonResponseData = "";
			try
			{
				request = request ?? new { };

				// Transformamos el REQUEST en formato JSON
				var jsonPostData = JsonConvert.SerializeObject(request);

				// Preparamos el cliente de la invocación WEB
				var webRequest = (HttpWebRequest)WebRequest.Create(url);
				webRequest.ContentType = "application/json; charset=utf-8";
				webRequest.Method = httpMethod.Method;

				// Asignamos el contenido de la invocación
				var postData = Encoding.UTF8.GetBytes(jsonPostData);
				webRequest.ContentLength = postData.Length;

				using (var stream = webRequest.GetRequestStream())
				{
					stream.Write(postData, 0, postData.Length);
					stream.Close();
				}

				using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
				{
					if (webResponse.StatusCode == HttpStatusCode.OK)
					{
						using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
						{
							jsonResponseData = streamReader.ReadToEnd();
						}
					}
					else
					{
						throw new ApplicationException(
							String.Format("Invocación a WEB API retornó código de estado: {0} - {1}.", webResponse.StatusCode, webResponse.StatusDescription));
					}
				}
			}
			catch (WebException ex)
			{
				jsonResponseData = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
			}
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			T dude = (!String.IsNullOrEmpty(jsonResponseData))
					? JsonConvert.DeserializeObject<T>(jsonResponseData, settings)
					: default(T);

			return dude;
		}
	}
}