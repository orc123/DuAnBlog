using DuAnBlog.Core.SeedWorks.Contants;
using System.Security.Claims;

namespace DuAnBlog.Api.Extensions;

public static class IdentityExtension
{
    public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
    {
        var claims = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

        return claims != null ? claims.Value : string.Empty;
    }

    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claim = ((ClaimsIdentity)principal.Identity).Claims.Single(x => x.Type == UserClaims.Id);
        return Guid.Parse(claim.Value);
    }
}
