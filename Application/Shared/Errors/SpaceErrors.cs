
using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class SpaceErrors
{
    public static Error NotFound =
        new Error("Space.NotFound", "The specified space was not found.");
    public static Error AlreadyExists =
        new Error("Space.AlreadyExists", "Space with the same name already exists.");
    public static Error AccessDenied =
        new Error("Space.AccessDenied", "You do not have permission to access these spaces.");
}
