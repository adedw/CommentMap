namespace CommentMap.Application.Models;

public record IdentityErrorDto(string Code, string Description);

public record IdentityResultDto(bool Succeeded, IReadOnlyList<IdentityErrorDto> Errors)
{
    public static IdentityResultDto Success() => new(true, []);

    public static IdentityResultDto Failed(IEnumerable<IdentityErrorDto> errors) =>
        new(false, [.. errors]);
}

public record LoginResultDto(
    bool Succeeded,
    bool RequiresTwoFactor,
    bool IsLockedOut,
    bool IsNotAllowed);

public record RegisterResultDto(
    bool Succeeded,
    bool RequireConfirmedAccount,
    IReadOnlyList<IdentityErrorDto> Errors);

public record AuthenticatorSetupDto(string SharedKey, string AuthenticatorUri, string QrCodeEmbedded);

public record EnableAuthenticatorResultDto(
    bool Succeeded,
    bool InvalidCode,
    bool ShowRecoveryCodes,
    string[]? RecoveryCodes,
    AuthenticatorSetupDto? Setup);
