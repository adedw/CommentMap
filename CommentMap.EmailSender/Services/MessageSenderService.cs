using CommentMap.EmailSender.Exceptions;
using CommentMap.EmailSender.Templates;
using Mjml.Net;

namespace CommentMap.EmailSender.Services;

public class MessageSenderService(ISmtpEmailSenderService smtpEmailSenderService, IMjmlRenderer mjmlRenderer)
    : IMessageSenderService
{
    public async Task SendConfirmationLinkAsync(string email, string callbackURL, CancellationToken ct = default)
    {
        var vm = new ConfirmEmailViewModel { CallbackURL = callbackURL };

        var html = await Render(vm, ct);

        await smtpEmailSenderService.SendHtmlEmailAsync(email, "Confirm your email", html, ct);
    }

    public async Task SendResetPasswordLinkAsync(string email, string callbackURL, CancellationToken ct = default)
    {
        var vm = new ResetPasswordViewModel { CallbackURL = callbackURL };

        var html = await Render(vm, ct);

        await smtpEmailSenderService.SendHtmlEmailAsync(email, "Confirm password reset", html, ct);
    }

    public async Task SendEmailChangingLinkAsync(string email, string callbackURL, CancellationToken ct = default)
    {
        var vm = new ChangeEmailViewModel { CallbackURL = callbackURL };

        var html = await Render(vm, ct);

        await smtpEmailSenderService.SendHtmlEmailAsync(email, "Confirm email changing", html, ct);
    }

    private async Task<string> Render(IEmailMessageViewModel vm, CancellationToken ct)
    {
        var view = new EmailMessageMjml(vm);

        var mjml = await view.RenderAsync(ct);

        var (html, errors) = await mjmlRenderer.RenderAsync(mjml, ct: ct);
        if (errors is { Count: > 0 })
        {
            throw new MjmlValidationException(errors);
        }
        return html;
    }
}
