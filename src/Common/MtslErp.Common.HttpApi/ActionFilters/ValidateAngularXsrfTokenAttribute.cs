using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.HttpApi.Others;

namespace MtslErp.Common.HttpApi.ActionFilters;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateAngularXsrfTokenAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var antiForgery = (IAntiforgery)context
            .HttpContext.RequestServices
            .GetRequiredService(typeof(IAntiforgery));

        if (await antiForgery.IsRequestValidAsync(context.HttpContext) is false)
        {
            context.Result = context.MakeResponse(StatusCodes.Status403Forbidden, "xsrf error");
            return;
        }

        await next();
    }
}
