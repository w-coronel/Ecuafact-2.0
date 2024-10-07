using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace Ecuafact.WebAPI
{
    /// <summary>
    /// Funciones Comunes
    /// </summary>
    public static class CommonFunctions
    {
        /// <summary>
        /// Creates an httpError object
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="userMessage"></param>
        /// <param name="devMessage"></param>
        /// <returns></returns>
        public static HttpError GetCustomHttpError(HttpStatusCode errorCode, string userMessage, string devMessage)
        {
            string httpErrorCode = ((int) errorCode).ToString();
            var success = errorCode < HttpStatusCode.MultipleChoices;
            var error = new HttpError(userMessage)
            {
                { "Id", Convert.ToInt32(success) },
                { "IsSuccess", success},
                { "UserMessage", userMessage },
                { "DevMessage", devMessage },
                { "StatusCode", httpErrorCode },
                { "MoreInfo", $"{Constants.WebApiUrl}/errors/{httpErrorCode}" }
            };
            return error;
        }

        /// <summary>
        /// Build a custom HttpResponseMessage that include additional information such as Developer Message, User Message and HttpStatusCode
        /// </summary>
        /// <param name="Request">the HttpRequestMessage</param>
        /// <param name="statusCode">Http Status Code</param>
        /// <param name="devMessage">Developer Message</param>
        /// <param name="userMessage">User Message</param>
        /// <returns></returns>
        public static HttpResponseMessage BuildHttpErrorResponse(this HttpRequestMessage Request, HttpStatusCode statusCode, string devMessage, string userMessage = null)
        {
            return Request.CreateErrorResponse(statusCode, GetCustomHttpError(statusCode, (string.IsNullOrWhiteSpace(userMessage) ? devMessage : userMessage), devMessage));
        }
         


        /// <summary>
        /// Build a custom HttpResponseException that include additional information such as Developer Message, User Message and HttpStatusCode
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="statusCode"></param>
        /// <param name="devMessage"></param>
        /// <param name="userMessage"></param>
        /// <returns></returns>
        public static HttpResponseException BuildHttpErrorException(this HttpRequestMessage Request, HttpStatusCode statusCode, string devMessage, string userMessage = null)
        {
            return new HttpResponseException(BuildHttpErrorResponse(Request, statusCode, devMessage, (string.IsNullOrWhiteSpace(userMessage) ? devMessage : userMessage)));
        }

        /// <summary>
        /// Build a custom HttpResponseException from a OperationResult object
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static HttpResponseException BuildHttpErrorException(this HttpRequestMessage Request, OperationResult result)
        {
            return new HttpResponseException(BuildHttpErrorResponse(Request, result.StatusCode, result.DevMessage, result.UserMessage));
        }

        /// <summary>
        /// Build a custom HttpResponseException from a OperationResult object
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static HttpResponseException ToErrorException(this OperationResult result, HttpRequestMessage Request)
        {
            return new HttpResponseException(BuildHttpErrorResponse(Request, result.StatusCode, result.DevMessage, result.UserMessage));
        }

        /// <summary>
        /// Build a custom HttpResponseException that include additional information such as User Message and HttpStatusCode
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="statusCode"></param>
        /// <param name="userMessage"></param>
        /// <returns></returns>
        public static HttpResponseException BuildHttpErrorException(this HttpRequestMessage Request, HttpStatusCode statusCode, string userMessage)
        {
            return new HttpResponseException(BuildHttpErrorResponse(Request, statusCode, userMessage, userMessage));
        }

        /// <summary>
        /// Convertir el objeto en Resultado de Operacion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="isSuccess"></param>
        /// <param name="statusCode"></param>
        /// <param name="userMessage"></param>
        /// <param name="devMessage"></param>
        /// <returns></returns>
        public static OperationResult<T> ToResult<T>(this T entity, bool isSuccess, HttpStatusCode statusCode, string userMessage = "OK", string devMessage = "")
        {
            return new OperationResult<T>(isSuccess, statusCode, entity)
            {
                DevMessage = devMessage,
                UserMessage = userMessage
            };
        }

    }

    /// <summary>
    /// Helper para los Ensamblados
    /// </summary>
    public static class AssemblyHelper
    {
        private static AssemblyName _assemblyName;
        private static Assembly _assembly;
        private static FileVersionInfo _fileVersion;

        private static DateTime _buildDate;
        static AssemblyHelper()
        {
            _assembly = Assembly.GetAssembly(typeof(AssemblyHelper));
            _assemblyName = _assembly.GetName();

            _fileVersion = FileVersionInfo.GetVersionInfo(_assembly.Location);
            _buildDate = new DateTime(2000, 1, 1)
                            .AddDays(AssemblyVersion.Build)
                            .AddSeconds(AssemblyVersion.Revision * 2);

        }

        /// <summary>
        /// Version Actual
        /// </summary>
        public static Version AssemblyVersion
        {
            get { return _assemblyName.Version; }
        }

        /// <summary>
        /// Fecha y Hora de Compilacion
        /// </summary>
        public static DateTime BuildDate
        {
            get { return _buildDate; }
        }

        /// <summary>
        /// Informacion de version del Archivo
        /// </summary>
        public static FileVersionInfo FileVersion
        {
            get { return _fileVersion; }
        }

        /// <summary>
        /// Nombre de la Compañia
        /// </summary>
        public static string Company
        {
            get
            {
                return FileVersion.CompanyName;
            }
        }

        /// <summary>
        /// Nombre del Producto
        /// </summary>
        public static string ProductName
        {
            get
            {
                return FileVersion.ProductName;
            }
        }

    }


}