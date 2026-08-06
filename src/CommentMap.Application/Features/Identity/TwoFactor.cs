using CommentMap.Application.Entities;
using CommentMap.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Text;
using System.Text.Encodings.Web;

namespace CommentMap.Application.Features.Identity;

public record GetTwoFactorStatus(Guid UserId);
public record TwoFactorStatusDto(
    bool Found,
    bool HasAuthenticator,
    bool Is2faEnabled,
    bool IsMachineRemembered,
    int RecoveryCodesLeft);

public static class GetTwoFactorStatusHandler
{
    public static async Task<TwoFactorStatusDto> Handle(
        GetTwoFactorStatus query,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new TwoFactorStatusDto(false, false, false, false, 0);

        return new TwoFactorStatusDto(
            true,
            await userManager.GetAuthenticatorKeyAsync(user) != null,
            await userManager.GetTwoFactorEnabledAsync(user),
            await signInManager.IsTwoFactorClientRememberedAsync(user),
            await userManager.CountRecoveryCodesAsync(user));
    }
}

public record ForgetTwoFactorClient(Guid UserId);

public static class ForgetTwoFactorClientHandler
{
    public static async Task<bool> Handle(
        ForgetTwoFactorClient command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return false;

        await signInManager.ForgetTwoFactorClientAsync();
        return true;
    }
}

public record GetAuthenticatorSetup(Guid UserId, string AppName = "CommentMap");

public static class GetAuthenticatorSetupHandler
{
    public static async Task<AuthenticatorSetupDto?> Handle(
        GetAuthenticatorSetup query,
        UserManager<User> userManager,
        UrlEncoder urlEncoder,
        QRCodeGenerator qrCodeGenerator)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return null;

        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        var sharedKey = FormatKey(unformattedKey!);
        var uri = GetQrCodeUri(urlEncoder, query.AppName, user.UserName!, unformattedKey!);
        var qr = GetEmbeddedSource(qrCodeGenerator, uri);
        return new AuthenticatorSetupDto(sharedKey, uri, qr);
    }

    internal static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
            result.Append(unformattedKey.AsSpan(currentPosition));

        return result.ToString().ToLowerInvariant();
    }

    internal static string GetQrCodeUri(UrlEncoder urlEncoder, string appName, string userName, string key)
    {
        var encodedAppName = urlEncoder.Encode(appName);
        var encodedUserName = urlEncoder.Encode(userName);
        return $"otpauth://totp/{encodedAppName}:{encodedUserName}?secret={key}&issuer={encodedAppName}&digits=6";
    }

    internal static string GetEmbeddedSource(QRCodeGenerator qrCodeGenerator, string qrCodeUri, int pixelsPerModule = 6)
    {
        using var data = qrCodeGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        using var base64 = new Base64QRCode(data);
        return $"data:image/png;base64,{base64.GetGraphic(pixelsPerModule)}";
    }
}

public record EnableAuthenticator(Guid UserId, string Code, string AppName = "CommentMap");

public static class EnableAuthenticatorHandler
{
    public static async Task<EnableAuthenticatorResultDto> Handle(
        EnableAuthenticator command,
        UserManager<User> userManager,
        ILogger<EnableAuthenticator> logger,
        UrlEncoder urlEncoder,
        QRCodeGenerator qrCodeGenerator)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return new EnableAuthenticatorResultDto(false, false, false, null, null);

        var verificationCode = command.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!isValid)
        {
            var setup = await GetAuthenticatorSetupHandler.Handle(
                new GetAuthenticatorSetup(command.UserId, command.AppName),
                userManager, urlEncoder, qrCodeGenerator);
            return new EnableAuthenticatorResultDto(false, true, false, null, setup);
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);

        if (await userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return new EnableAuthenticatorResultDto(true, false, true, recoveryCodes!.ToArray(), null);
        }

        return new EnableAuthenticatorResultDto(true, false, false, null, null);
    }
}

public record Disable2fa(Guid UserId);

public static class Disable2faHandler
{
    public static async Task<bool> Handle(Disable2fa command, UserManager<User> userManager, ILogger<Disable2fa> logger)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return false;

        if (!await userManager.GetTwoFactorEnabledAsync(user))
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");

        var result = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!result.Succeeded)
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");

        logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", user.Id);
        return true;
    }
}

public record ResetAuthenticator(Guid UserId);

public static class ResetAuthenticatorHandler
{
    public static async Task<bool> Handle(
        ResetAuthenticator command,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<ResetAuthenticator> logger)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return false;

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);
        logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);
        await signInManager.RefreshSignInAsync(user);
        return true;
    }
}

public record GenerateRecoveryCodes(Guid UserId);
public record GenerateRecoveryCodesResult(bool Found, bool TwoFactorEnabled, string[]? RecoveryCodes);

public static class GenerateRecoveryCodesHandler
{
    public static async Task<GenerateRecoveryCodesResult> Handle(
        GenerateRecoveryCodes command,
        UserManager<User> userManager,
        ILogger<GenerateRecoveryCodes> logger)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return new GenerateRecoveryCodesResult(false, false, null);

        if (!await userManager.GetTwoFactorEnabledAsync(user))
            throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.");

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", user.Id);
        return new GenerateRecoveryCodesResult(true, true, recoveryCodes!.ToArray());
    }
}

public record LoginWith2fa(string TwoFactorCode, bool RememberMe, bool RememberMachine);
public record LoginWithRecoveryCode(string RecoveryCode);

public static class LoginWith2faHandler
{
    public static async Task<LoginResultDto> Handle(
        LoginWith2fa command,
        SignInManager<User> signInManager,
        ILogger<LoginWith2fa> logger)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync()
            ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        var authenticatorCode = command.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(
            authenticatorCode, command.RememberMe, command.RememberMachine);

        if (result.Succeeded)
            logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
        else if (result.IsLockedOut)
            logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

        return result.ToDto();
    }
}

public static class LoginWithRecoveryCodeHandler
{
    public static async Task<LoginResultDto> Handle(
        LoginWithRecoveryCode command,
        SignInManager<User> signInManager,
        ILogger<LoginWithRecoveryCode> logger)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync()
            ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        var recoveryCode = command.RecoveryCode.Replace(" ", string.Empty);
        var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded)
            logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
        else if (result.IsLockedOut)
            logger.LogWarning("User account locked out.");

        return result.ToDto();
    }
}

public record SignInAfterRegistration(Guid UserId);

public static class SignInAfterRegistrationHandler
{
    public static async Task Handle(SignInAfterRegistration command, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString())
            ?? throw new InvalidOperationException("User not found.");
        await signInManager.SignInAsync(user, isPersistent: false);
    }
}
