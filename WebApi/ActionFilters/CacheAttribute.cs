using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.ActionFilters;

public class CacheAttribute(int TimeToLiveInSec = 90) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        var key = CreateKey(context.HttpContext.Request);

        var cachedValue = cacheService.GetData(key);

        if (!string.IsNullOrEmpty(cachedValue))
        {
            context.Result = new ContentResult
            {
                Content = cachedValue,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK
            };
            return;
        }
        var executedContext = await next.Invoke();
        if (executedContext.Result is OkObjectResult result)
        {
            cacheService.SetData(key, result.Value, TimeToLiveInSec);
        }
    }

    private string CreateKey(HttpRequest request)
    {
        var path = request.Path.ToString();
        var queryStrings = request.Query.OrderBy(x => x.Key);
        var key = $"{path}?" + string.Join("&", queryStrings.Select(x => $"{x.Key}={x.Value}"));
        return key;
    }
}

