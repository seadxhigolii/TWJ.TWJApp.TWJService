using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace TWJ.TWJApp.TWJService.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BadRequestException) context.ExceptionFilerResponse(HttpStatusCode.BadRequest);
        }
    }
}
