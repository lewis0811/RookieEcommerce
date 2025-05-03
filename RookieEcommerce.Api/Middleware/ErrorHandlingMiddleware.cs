using FluentValidation;
using System.Net;

namespace RookieEcommerce.Api.Middleware
{
    public class ErrorHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        // Process thourgh the context to check if there are any exceptions
        public async Task InvokeAsync(HttpContext context)
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

        // Handle the exception and translate it into a specific exception type
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode;
            object? responsePayload = null;

            switch (ex)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    responsePayload = new
                    {
                        error = "An validation error occurred.",
                        details = validationException.Errors.Select(c => $"{c.ErrorMessage}")
                    };
                    break;

                case InvalidOperationException invalidOperationException:
                    statusCode = HttpStatusCode.NotFound;
                    responsePayload = new
                    {
                        error = "An invalid operation error occurred.",
                        details = invalidOperationException.Message
                    };
                    break;

                case ArgumentException argumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    responsePayload = new
                    {
                        error = "An argument error occurred.",
                        details = argumentException.Message
                    };
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    // Avoid leaking sensitive details in production
#if DEBUG
                    // Include more detail in DEBUG builds if desired
                    responsePayload = new
                    {
                        error = "An internal server error occurred.",
                        details = ex.Message
                    };
#else
                    responsePayload = new { error = "An internal server error occurred." };
#endif
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            // Write the response payload as JSON
            if (responsePayload != null)
            {
                await context.Response.WriteAsJsonAsync(responsePayload);
            }
        }
    }
}