using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.PayMe
{
    public static class PaymentezService
    {
        private static string server_application_code = "";
        private static string server_app_key = "";
        public static string PAYMENTEZ_URL = "";       
        public static string RESPONSE_HTTP_CODE = "RESPONSE_HTTP_CODE";
        public static string RESPONSE_JSON = "RESPONSE_JSON";
      

        private static string getUniqToken(string auth_timestamp, string app_secret_key)
        {
            string uniq_token_string = app_secret_key + auth_timestamp;
            return GetHashSha256(uniq_token_string);
        }

        public static string getAuthToken(string app_code, string app_secret_key)
        {
            string auth_timestamp = "" + (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            string string_auth_token = app_code + ";" + auth_timestamp + ";" + getUniqToken(auth_timestamp, app_secret_key);
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(string_auth_token);
            string auth_token = Convert.ToBase64String(toEncodeAsBytes);
            return auth_token;
        }

        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;

            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }

        public static string paymentezVerifyJson(string transaction_id)
        {
            return "{" +                       
                        "\"transaction\": {" +
                            "\"id\": \"" + transaction_id + "\"" +
                        "}"+
                    "}";
        }

        public static Dictionary<string, string> doPostRequest(this ECommerce commerce, string id)
        {


            //RequestBody body = RequestBody.create(JSON, json);
            // turn our request string into a byte stream
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(commerce.UrlApiRestService);
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json";
            request.Method = "POST";
            request.Headers.Add("Auth-Token", PaymentezService.getAuthToken(commerce.ServerAppCode, commerce.ServerAppKey));

            StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());

            JObject o = JObject.Parse(paymentezVerifyJson(id));
            streamWriter.Write(o);
            streamWriter.Flush();

            Dictionary<string, string> mapResult = new Dictionary<string, string>();
            try
            {
                HttpWebResponse reponse = (HttpWebResponse)request?.GetResponse();
                mapResult.Add(RESPONSE_HTTP_CODE, "" + reponse?.StatusCode);
                using (var streamReader = new StreamReader(reponse?.GetResponseStream()))
                {
                    mapResult.Add(RESPONSE_JSON, streamReader.ReadToEnd());
                }
            }
            catch (WebException ex)
            {
                using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    mapResult.Add(RESPONSE_JSON, streamReader?.ReadToEnd());
                }
            }

            streamWriter.Close();
            return mapResult;
        }
    }
}
