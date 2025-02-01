using DuAnBlog.Core.Domain.Identity;
using DuAnBlog.Core.SeedWorks.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DuAnBlog.Api.Authorization;

public class PermissionAuthorizationHandler(RoleManager<AppRole> roleManager,
    UserManager<AppUser> userManager) : AuthorizationHandler<PermissionRequirement>
{

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity.IsAuthenticated == false)
        {
            return;
        }
        var user = await userManager.FindByNameAsync(context.User.Identity.Name);
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(Roles.Admin))
        {
            context.Succeed(requirement);
            return;
        }
        var allPermissions = new List<Claim>();
        foreach (var role in roles)
        {
            var roleEntity = await roleManager.FindByNameAsync(role);
            var roleClaims = await roleManager.GetClaimsAsync(roleEntity);
            allPermissions.AddRange(roleClaims);

        }
        var permissions = allPermissions.Where(x => x.Type == "Permission" &&
                                                            x.Value == requirement.Permission &&
                                                            x.Issuer == "LOCAL AUTHORITY");
        if (permissions.Any())
        {
            context.Succeed(requirement);
            return;
        }
    }
}