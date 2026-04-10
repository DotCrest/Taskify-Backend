using Application.Common.Errors;

namespace Application.Shared.Errors
{
    public static class CategoryErrors
    {
        public static readonly Error NotFound
            = new("Category.NotFound", "The specified category was not found.");
        public static readonly Error NotInWorkspace
            = new("Category.NotInWorkspace", "The specified category does not belong to the workspace.");
    }
}
