using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MtslErp.Common.HttpApi.Others;

public static class ControllerExtensions
{
    public static IActionResult MakeResponse(this ActionContext context, int code, object? data = null)
    {
        if (data is null)
        {
            return new StatusCodeResult(code);
        }

        return new JsonResult(data) { StatusCode = code };
    }

    public static IActionResult MakeDynamicQueryResponse<T>(this ActionContext context,
        IEnumerable<T> dataFromDb, long count, int pageSize)
    {
        if (pageSize <= 0)
        {
            pageSize = 1;
        }

        var totalPages = (long)Math.Ceiling(count / (decimal)pageSize);
        return new JsonResult(new { data = dataFromDb, last_row = count, last_page = totalPages })
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public static IActionResult MakeValidationErrorResponse(this ActionContext context)
    {
        var errors = context.ModelState
            .Where(e => e.Value is { Errors.Count: > 0 })
            .Select(e => new
            {
                Field = JsonNamingPolicy.CamelCase.ConvertName(e.Key),
                Errors = e.Value?.Errors.Select(er => er.ErrorMessage)
            });

        return context.MakeResponse(StatusCodes.Status400BadRequest, errors);
    }
}
