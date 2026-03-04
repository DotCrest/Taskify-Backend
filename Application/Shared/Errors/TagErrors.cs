using Application.Common.Errors;

namespace Application.Shared.Errors;

public class TagErrors
{
    public static Error AccessDenied =
        new Error("Tag.AccessDenied", "You do not have permission to create or access tags.");
}
