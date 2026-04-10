using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs.HubFilters;

public class CommentHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {

        var arguments = invocationContext.HubMethodArguments.ToList();
        foreach (var arg in arguments)
        {
            if (arg is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
            var validator = invocationContext.ServiceProvider.GetService(validatorType) as IValidator;
            if (validator is not null)
            {
                var context = new ValidationContext<object>(arg);
                var validationResult = await validator.ValidateAsync(context);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(" | ", validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                    throw new HubException(errors);
                }
            }
        }
        return await next(invocationContext);
    }
}