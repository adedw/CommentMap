namespace CommentMap.Application.Models;

public record IdentityErrorDto(string Code, string Description);

public enum IdentityFailure
{
    None,
    UserNotFound,
    IncorrectPassword,
}

public record IdentityResultDto(bool Succeeded, IdentityFailure Failure, IReadOnlyList<IdentityErrorDto> Errors)
{
    public static IdentityResultDto Success() => new(true, IdentityFailure.None, []);

    public static IdentityResultDto Failed(IEnumerable<IdentityErrorDto> errors) =>
        new(false, IdentityFailure.None, [.. errors]);

    public static IdentityResultDto UserNotFound() =>
        new(false, IdentityFailure.UserNotFound, [new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

    public static IdentityResultDto IncorrectPassword() =>
        new(false, IdentityFailure.IncorrectPassword, [new IdentityErrorDto("Password", "Incorrect password.")]);
}

public record LoginResultDto(
    bool Succeeded,
    bool RequiresTwoFactor,
    bool IsLockedOut,
    bool IsNotAllowed);

public record AuthenticatorSetupDto(string SharedKey, string AuthenticatorUri, string QrCodeEmbedded);

public record EnableAuthenticatorResultDto(
    bool Succeeded,
    bool InvalidCode,
    bool ShowRecoveryCodes,
    string[]? RecoveryCodes,
    AuthenticatorSetupDto? Setup);
