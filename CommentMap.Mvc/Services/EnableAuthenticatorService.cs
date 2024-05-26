using QRCoder;
using System.Text.Encodings.Web;

namespace CommentMap.Mvc.Services;

public class EnableAuthenticatorService(UrlEncoder urlEncoder, QRCodeGenerator qRCodeGenerator)
    : IEnableAuthenticatorService
{
    public string GetQrCodeUri(string appName, string userName, string key)
    {
        var encodedAppName = urlEncoder.Encode(appName);
        var encodedUserName = urlEncoder.Encode(userName);
        return $"otpauth://totp/{encodedAppName}:{encodedUserName}?secret={key}&issuer={encodedAppName}&digits=6";
    }

    public string GetEmbeddedSource(string qrCodeUri, int pixelsPerModule = 6)
    {
        using var qRCodeData = qRCodeGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        using var base64QRCode = new Base64QRCode(qRCodeData);

        var base64EncodedQrCode = base64QRCode.GetGraphic(pixelsPerModule);
        return $"data:image/png;base64,{base64EncodedQrCode}";
    }
}
