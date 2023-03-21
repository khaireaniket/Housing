using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Housing.API.Filters
{
    /// <summary>
    /// APIException filter
    /// </summary>
    public class APIExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Invoked when unhandled exception occurs in the application
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "OOPS! An error occurred while processing your request."
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;

            base.OnException(context);
        }
    }
}
