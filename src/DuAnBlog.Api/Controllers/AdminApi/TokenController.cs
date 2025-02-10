using DuAnBlog.Api.Services;
using DuAnBlog.Core.Domain.Identity;
using DuAnBlog.Core.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DuAnBlog.Api.Controllers.AdminApi;
[Route("api/admin/[controller]")]
[ApiController]
public class TokenController(UserManager<AppUser> userManager, ITokenService tokenService) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;
    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult<AuthenticatedResult>> Refresh(TokenRequest tokenRequest)
    {
        if (tokenRequest == null)
        {
            return BadRequest("Invalid client request");
        }
        string accessToken = tokenRequest.AccessToken;
        string refreshToken = tokenRequest.RefreshToken;
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return BadRequest("Invalid token");
        }
        var username = principal.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid client request");
        }
        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return Ok(new AuthenticatedResult
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [HttpPost]
    [Authorize]
    [Route("revoke")]
    public async Task<ActionResult> Revoke()
    {
        var username = User.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return BadRequest();
        }
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);
        return NoContent();
    }
}
