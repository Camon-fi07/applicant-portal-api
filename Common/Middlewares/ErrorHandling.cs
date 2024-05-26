using Common.Models;
using Microsoft.AspNetCore.Http;

namespace Common.Middlewares
{
    public class ErrorsHandling
    {
        private readonly RequestDelegate _next;

        public ErrorsHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomExceptionModel e)
            {
                context.Response.StatusCode = e.ErrorCode;
                await context.Response.WriteAsJsonAsync(new { message = e.Message });
            }
        }
    }
}
