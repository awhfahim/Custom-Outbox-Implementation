using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.HttpApi.Others;

namespace MtslErp.Common.HttpApi.ActionFilters;

[AttributeUsage(AttributeTargets.Method)]
public class ValidationActionFilterAttribute<T> : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid is false)
        {
            context.Result = context.MakeValidationErrorResponse();
            return;
        }

        var validator = (IValidator<T>)context
            .HttpContext.RequestServices
            .GetRequiredService(typeof(IValidator<T>));

        if (context.ActionArguments.FirstOrDefault(x => x.Value?.GetType() == typeof(T)).Value is not T
            instance)
        {
            context.Result = context.MakeResponse(StatusCodes.Status400BadRequest);
            return;
        }

        var validationResult = await validator.ValidateAsync(instance);
        if (validationResult.IsValid is false)
        {
            validationResult.AddToModelState(context.ModelState);
            context.Result = context.MakeValidationErrorResponse();
            return;
        }

        await next();
    }
}
