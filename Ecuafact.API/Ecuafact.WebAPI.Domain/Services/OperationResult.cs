using System;
using System.Net;

namespace Ecuafact.WebAPI.Domain.Services
{
    /// <summary>
    /// Resultado de la Operación
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Código del Estado HTTP de la operación
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Si el proceso se realizo exitosamente
        /// </summary>
        public bool IsSuccess { get; private set; }
        
        /// <summary>
        /// Mensaje con informacion relevante para el desarrollador
        /// </summary>
        public string DevMessage { get; set; }
        
        /// <summary>
        /// Mensaje del resultado de la operación
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Mas informacion sobre el resultado
        /// </summary>
        public string MoreInfo => $"{Constants.WebApiUrl}/errors/{StatusCode}";

        
        public OperationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public OperationResult(bool isSuccess, HttpStatusCode statusCode) : this(isSuccess)
        {
            StatusCode = statusCode;
        }
        public OperationResult(bool isSuccess, HttpStatusCode statusCode, string message) : this(isSuccess, statusCode)
        {
            DevMessage = UserMessage = message;
        }

        public OperationResult(Exception ex) : this(ex, HttpStatusCode.InternalServerError) { }

        public OperationResult(Exception ex, HttpStatusCode statusCode)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            UserMessage = ex.Message;
            DevMessage = ex.ToString();
        }
    }

    public class OperationResult<TEntity> : OperationResult
    {
        public OperationResult(OperationResult result) : base(result.IsSuccess, result.StatusCode, result.UserMessage)
        {
            DevMessage = result.DevMessage;
        }

        public OperationResult(Exception ex) : base(ex) { }

        public OperationResult(Exception ex, HttpStatusCode statusCode) : base(ex, statusCode) { }

        public OperationResult(bool isSuccess, HttpStatusCode statusCode) : base(isSuccess, statusCode)
        {
        }

        public OperationResult(bool isSuccess, HttpStatusCode statusCode, string message) : base(isSuccess, statusCode, message)
        {
        }

        public OperationResult(bool isSuccess, HttpStatusCode statusCode, TEntity entity) : base(isSuccess, statusCode)
        {
            Entity = entity;
        }

        /// <summary>
        /// Objeto Resultado de la Operación
        /// </summary>
        public TEntity Entity { get; set; }
    }
}
