namespace Camply.Api.Models.Responses
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }

        public static ApiResponse Ok(string message = "")
        {
            return new ApiResponse { Success = true, Message = message };
        }

        public static ApiResponse Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse { Success = false, Message = message, Errors = errors };
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "")
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message };
        }
    }

}