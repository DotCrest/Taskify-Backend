using Application.Common.Errors;

namespace Application.Shared.Errors;

public class TagErrors
{
    public static Error AccessDenied =
        new Error("Tag.AccessDenied", "You do not have permission to create or access tags.");
    public static Error AlreadyExisted =
        new Error("Tag.AlreadyExisted", "A tag with the same name already exists in this workspace.");
    public static Error NotFound =
        new Error("Tag.NotFound", "The specified tag was not found.");
}
