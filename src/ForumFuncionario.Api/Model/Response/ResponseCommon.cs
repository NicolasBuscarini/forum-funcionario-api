using System.Text.Json.Serialization;

namespace ForumFuncionario.Api.Model.Response
{
    public class ResponseCommon<T>
    {
        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string? Error { get; set; }

        public T Data { get; set; }

        [JsonConstructor]
        public ResponseCommon()
        {
        }

        private ResponseCommon(int statusCode, string message, T data, string? error = null, bool isSuccess = false)
        {
            StatusCode = statusCode;
            Message = message;
            Error = error;
            Data = data;
            IsSuccess = isSuccess;
        }

        public static ResponseCommon<T> Success(T data, string message = "Success")
        {
            return new ResponseCommon<T>(200, message, data, null, isSuccess: true);
        }

        public static ResponseCommon<T> Failure(string message, int statusCode = 500, string? error = null)
        {
            return new ResponseCommon<T>(statusCode, message, default(T), error);
        }
    }
}
