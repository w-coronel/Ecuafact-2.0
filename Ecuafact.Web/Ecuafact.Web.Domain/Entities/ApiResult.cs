using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace Ecuafact.Web.Domain.Entities
{
    public class ApiResult 
    {
        public ApiResult(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public int Id { get; set; } = 0;
        public string Message { get; set; } = "";
        public string Error { get; set; } = "ERROR";
        public HttpStatusCode StatusCode { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        } 

        public HttpResponseMessage ToResponseMessage()
        {
            return new HttpResponseMessage(StatusCode) { Content = new StringContent(ToString()) };
        }
    }


    public class ErrorResult
    {
        public string Id { get; set; }
        public string UserMessage { get; set; }
        public string DeveloperMessage { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
        public string MoreInfo { get; set; }
    }
}
