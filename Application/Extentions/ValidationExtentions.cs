using Application.Common.Errors;
using Application.Shared;

namespace Application.Extentions
{
    public static class ValidationExtentions
    {
        public static Result<T> ToFailure<T>(this FluentValidation.Results.ValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.PropertyName, e.ErrorMessage))
                .ToList();
            return Result<T>.Failure(errors);


        }
    }
}
