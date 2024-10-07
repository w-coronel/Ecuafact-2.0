using System;
using System.Net;

namespace Ecuafact.Web.Domain.Services
{
    public class OperationResult
    {
        public OperationResult() { }
        public OperationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public OperationResult(Exception ex) : this(ex, HttpStatusCode.InternalServerError) { }

        public OperationResult(Exception ex, HttpStatusCode statusCode)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            UserMessage = ex.Message;
            DevMessage = ex.ToString();
        }

        public OperationResult(bool isSuccess, HttpStatusCode statusCode, string message = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            UserMessage = message;
        }

        public HttpStatusCode StatusCode { get; set; } 
        public bool IsSuccess { get; set; } 
        public string DevMessage { get; set; }
        public string UserMessage { get; set; }
        public string MoreInfo { get; set; }
    }

    public class OperationResult<TEntity> : OperationResult
    {
        public OperationResult() : base() { }
        public OperationResult(OperationResult operationResult)
        {
            this.IsSuccess = operationResult.IsSuccess;
            this.MoreInfo = operationResult.MoreInfo;
            this.StatusCode = operationResult.StatusCode;
            this.UserMessage = operationResult.UserMessage;
            this.DevMessage = operationResult.DevMessage;
        }

        public OperationResult(bool isSuccess) : base(isSuccess) { }
        public OperationResult(Exception ex) : base(ex) {  }
        public OperationResult(Exception ex, HttpStatusCode statusCode) : base(ex, statusCode) { }
        public OperationResult(bool isSuccess, HttpStatusCode statusCode) : base(isSuccess, statusCode) { }

        public OperationResult(bool isSuccess, HttpStatusCode statusCode, TEntity entity) : base(isSuccess, statusCode)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }

    }
}
