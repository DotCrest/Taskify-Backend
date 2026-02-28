using Application.Common.Errors;
using Application.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected async Task<Result<bool>> ExecuteWithValidation<T>(IValidator<T> validator, T dto)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(Code: e.PropertyName, Message: e.ErrorMessage)).ToList();
            return Result<bool>.Failure(errors);
        }
        return Result<bool>.Success(true);
    }
    protected ActionResult HandleFailure(IEnumerable<Error> errors)
    {
        var firstError = errors?.FirstOrDefault();
        var errorCode = firstError?.Code ?? string.Empty;

        var actionResult = errorCode switch
        {
            _ when errorCode.Contains("NotFound") => NotFound(errors),
            _ when errorCode.Contains("AlreadyExist") => Conflict(errors),
            _ when errorCode.Contains("LockedOut") => StatusCode(StatusCodes.Status423Locked, errors),
            _ when errorCode.Contains("AccessDenied") => StatusCode(StatusCodes.Status403Forbidden, errors),
            _ when errorCode.Contains("InvalidCredentials") || errorCode.Contains("InvalidRefreshToken") ||
            errorCode.Contains("Unauthorized")
                => Unauthorized(errors),
            _ => BadRequest(errors)
        };
        return actionResult;
    }
}
