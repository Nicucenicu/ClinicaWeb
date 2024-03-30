using ClinicaWeb.Shared.ExceptionHandlingModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClinicaWeb.Application.ExceptionHandling
{
    public class CustomExceptionFilter : IExceptionFilter
    {

        public virtual void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case BusinessException exception:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Result = new JsonResult(new ErrorResponse(exception.FailureReason, exception.Message));
                    break;
                case RequestValidationFailedException exception:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Result = new JsonResult(new ErrorResponse(FailureReason.ValidationErrors, exception.Failures));
                    break;
                default:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Result = new JsonResult(new ErrorResponse { Message = $"{context.Exception?.Message} {context.Exception?.InnerException?.Message}" });
                    break;
            }
        }

    }

}
