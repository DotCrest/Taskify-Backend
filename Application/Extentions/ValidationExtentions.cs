using Application.Common.Errors;
using Application.Shared;
using FluentValidation.Results;

namespace Application.Extentions
{
    public static class ValidationExtentions
    {
        public static Result<T> ToFailure<T>(this ValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.PropertyName, e.ErrorMessage))
                .ToList();
            return Result<T>.Failure(errors);


        }
    }
}
