
using BankingService.Core.Exceptions;
using NLog;

namespace BankingService.Api.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BusinessException ex)
            {
                logger.Error(ex);
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal Server Error");
            }
        }
    }
}
