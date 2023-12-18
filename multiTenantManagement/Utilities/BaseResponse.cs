using System.Net;

namespace multiTenantManagement.Utilities
{
    public partial class BaseResponse<T>
    {
        public T Data { get; set; }
        public string ResponseMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
       
        public object Error { get; set; }

        public BaseResponse()
        {
            
        }

        public BaseResponse(T data, HttpStatusCode statusCode, string message = "Success")
        {
            Data = data;
            ResponseMessage = message;
            StatusCode = statusCode;
        }

        public BaseResponse(string message, HttpStatusCode statusCode, object errors = null)
        {
            ResponseMessage = message;
            StatusCode = statusCode;
            Error = errors;
        }

    }
}
