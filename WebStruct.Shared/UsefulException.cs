using System.Net;

namespace WebStruct.Shared
{
    public class UsefulException : Exception
    {
        public int StatusCodeHTTP { get; set; }
        public string[] Errors { get; set; }

        public UsefulException(HttpStatusCode statusCode, string[] errors, string? exMessage = "Возникли ошибки", Exception? innerException = default) : base(exMessage, innerException)
        {
            StatusCodeHTTP = (int)statusCode;
            Errors = errors;
        }
    }
}
