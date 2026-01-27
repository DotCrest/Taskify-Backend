using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.ErrorFactory("Auth.InvalidCredentials", "Invalid Email Or Password");
    public static readonly Error UserLockedOut =
        Error.ErrorFactory("Auth.UserLockedOut", "Account Is Locked due to many failed attempts");
    public static readonly Error EmailAlreadyExsists =
        Error.ErrorFactory("Auth.EmailAlreadyExsist", "User with this email  already exsisit");
    public static readonly Error UsernameAlreadyExsists =
        Error.ErrorFactory("Auth.UsernameAlreadyExsist", "User with this username already exists");
    public static readonly Error InvalidRefreshToken =
        Error.ErrorFactory("Auth.InvalidRefreshToken", "The provided RefreshToken token is invalid or has expired");
}
