namespace Api.ApiResDto
{
    public class ApiResponse<T>
    {
        public int? StatusCode { get; set; } = 200; // Default to 200 OK
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(int? satusCode = 200, bool success, string? message = null, T? data = default)
        {
            StatusCode = satusCode;
            Success = success;
            Message = message;
            Data = data;
        }

        public ApiResponse(bool success, string? message = null, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
