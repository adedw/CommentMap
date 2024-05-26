namespace CommentMap.Mvc.Services;

public interface IEnableAuthenticatorService
{
    string GetQrCodeUri(string appName, string userName, string key);
    string GetEmbeddedSource(string qrCodeUri, int pixelsPerModule = 6);
}
