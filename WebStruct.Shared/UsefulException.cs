using System.Net;

namespace WebStruct.Shared
{
    /// <summary>
    /// Исключение со статус-кодом и сообщением об ошибке для удобной обработки на слоях.
    /// </summary>
    public class UsefulException : Exception
    {
        public int StatusCodeHTTP { get; set; }

        public UsefulException(HttpStatusCode statusCode, string exMessage, Exception? innerException = default) : base(exMessage, innerException)
        {
            StatusCodeHTTP = (int)statusCode;
        }
    }
}
