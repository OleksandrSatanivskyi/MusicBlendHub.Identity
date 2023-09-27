using Newtonsoft.Json;
using System.Net;

namespace MusicBlendHub.Identity.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        public readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            var errorResponse = new { Message = exception.Message};

            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
