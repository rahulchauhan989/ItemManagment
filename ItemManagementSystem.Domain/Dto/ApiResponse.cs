namespace ItemManagementSystem.Domain.Dto;

  public class ApiResponse
    {
        public bool IsSuccess { get; set; } = true;
        public int StatusCode { get; set; } = 200; 
        public object? Data { get; set; }
        // public List<string> ErrorMessages { get; set; } = new List<string>();

        public string? Message { get; set; }

        public ApiResponse(bool isSuccess = true, int statusCode = 200, object? data = null, string? message = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }
    }