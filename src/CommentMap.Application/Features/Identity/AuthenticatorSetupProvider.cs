using System.Text;
using System.Text.Encodings.Web;

using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using QRCoder;

namespace CommentMap.Application.Features.Identity;

public interface IAuthenticatorSetupProvider
{
    Task<AuthenticatorSetupDto?> CreateAsync(User user, CancellationToken cancellationToken);
}

public sealed class AuthenticatorSetupProvider(
    UserManager<User> userManager,
    UrlEncoder urlEncoder,
    QRCodeGenerator qrCodeGenerator,
    IOptions<AuthenticatorOptions> options) : IAuthenticatorSetupProvider
{
    public async Task<AuthenticatorSetupDto?> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        var sharedKey = FormatKey(unformattedKey!);
        var uri = GetQrCodeUri(urlEncoder, options.Value.AppName, user.UserName!, unformattedKey!);
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
