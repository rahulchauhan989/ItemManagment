using System.Net;
using System.Text.Json;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Exception;

namespace ItemManagementSystem.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                 var ipAddress = context.Connection.RemoteIpAddress?.ToString();                 
                 Console.WriteLine("IpAdress : " + ipAddress);
                 await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
        {
            var innerException = exception.InnerException;
            var error = $"Exception is thrown ({exception.GetType()}): {exception.Message}\n";

            var errorMessages = new List<string>();
            if (innerException != null)
            {
                error += $"Inner Exception ({innerException.GetType()}) : {innerException.Message}";
                errorMessages.Add(innerException.Message);
            }

            Console.WriteLine("inner Exception:" + error);

            HttpStatusCode statusCode = exception switch
            {
                CustomException => HttpStatusCode.BadRequest,
                NullObjectException => HttpStatusCode.NotFound,
                AlreadyExistsException => HttpStatusCode.Conflict,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new ApiResponse()
            {
                StatusCode =(int) statusCode,
                Message = exception.Message,
                IsSuccess = false,
                Data = null,
            });

            return context.Response.WriteAsync(result);
        }
    }
}