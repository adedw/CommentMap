using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

internal static class IdentityMapping
{
    public static IdentityResultDto ToDto(this IdentityResult result) =>
        new(result.Succeeded, result.Errors.Select(e => new IdentityErrorDto(e.Code, e.Description)).ToList());

    public static LoginResultDto ToDto(this SignInResult result) =>
        new(result.Succeeded, result.RequiresTwoFactor, result.IsLockedOut, result.IsNotAllowed);
}
