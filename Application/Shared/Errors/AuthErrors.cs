using Application.Common.Errors;

namespace Application.Shared.Errors
{
    public static class AuthErrors
    {
        public static readonly Error InvalidCredentials =
            new Error("Auth.InvalidCredentials", "Invalid Email Or Password");
        public static readonly Error UserLockedOut =
            new Error("Auth.UserLockedOut", "Account Is Locked due to many failed attempts");
        public static readonly Error EmailAlreadyExsists =
            new Error("Auth.EmailAlreadyExsist", "User with this email  already exsisit");
        public static readonly Error UsernameAlreadyExsists =
            new Error("Auth.UsernameAlreadyExsist", "User with this username already exists");
        public static readonly Error InvalidRefreshToken =
            new Error("Auth.InvalidRefreshToken", "The refresh token is invalid");
        public static readonly Error UserNotFound =
            new Error("Auth.UserNotFound", "User not found");
    }
}
