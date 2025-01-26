using DuAnBlog.Api.Extensions;
using DuAnBlog.Api.Services;
using DuAnBlog.Core.Domain.Identity;
using DuAnBlog.Core.Models.Auth;
using DuAnBlog.Core.Models.System;
using DuAnBlog.Core.SeedWorks.Contants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace DuAnBlog.Api.Controllers.AdminApi;
[Route("api/admin/[controller]")]
[ApiController]
public class AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, RoleManager<AppRole> roleManager) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly RoleManager<AppRole> _roleManager = roleManager;

    [HttpPost]
    public async Task<ActionResult<AuthenticatedResult>> Login([FromBody] LoginRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request");
        }

        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !user.IsActive || user.LockoutEnabled)
        {
            return Unauthorized();
        }
        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await GetPermissionsByUserIdAsync(user.Id.ToString());

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(UserClaims.Id, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(UserClaims.FirstName, user.FirstName),
            new Claim(UserClaims.Roles, string.Join(";", roles)),
            new Claim(UserClaims.Permissions, JsonSerializer.Serialize(permissions)),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(30);
        await _userManager.UpdateAsync(user);

        return Ok(new AuthenticatedResult()
        {
            Token = accessToken,
            RefreshToken = refreshToken
        });
    }

    private async Task<List<string>> GetPermissionsByUserIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<string>();
        }
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        var allPermissions = new List<RoleClaimsDto>();
        if (roles.Contains(Roles.Admin))
        {
            var types = typeof(Permissions).GetTypeInfo().DeclaredNestedTypes;
            foreach (var type in types)
            {
                allPermissions.GetPermissions(type);
            }
            permissions.AddRange(allPermissions.Select(x => x.Value));
        }
        else
        {
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var claims = await _roleManager.GetClaimsAsync(role);
                var roleClaimValues = claims.Select(x => x.Value).ToList();
                permissions.AddRange(roleClaimValues);
            }
        }
        return permissions.Distinct().ToList();
    }
}