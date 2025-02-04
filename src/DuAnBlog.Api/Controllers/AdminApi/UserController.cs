using AutoMapper;
using DuAnBlog.Api.Extensions;
using DuAnBlog.Api.Filters;
using DuAnBlog.Core.Domain.Identity;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Auth;
using DuAnBlog.Core.SeedWorks.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static DuAnBlog.Core.SeedWorks.Contants.Permissions;

namespace DuAnBlog.Api.Controllers.AdminApi;
[Route("api/admin/user")]
[ApiController]
public class UserController(IMapper mapper, UserManager<AppUser> userManager) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(Users.View)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        var userDto = mapper.Map<AppUser, UserDto>(user);
        var roles = await userManager.GetRolesAsync(user);
        userDto.Roles = roles;

        return Ok(userDto);
    }

    [HttpGet("paging")]
    [Authorize(Users.View)]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAllUsersPaging(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = userManager.Users;
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.FirstName.Contains(keyword) || 
                                    x.UserName.Contains(keyword) || 
                                    x.Email.Contains(keyword) || 
                                    x.PhoneNumber.Contains(keyword));
        }
        var totalRow = await query.CountAsync();
        query = query.OrderByDescending(x => x.DateCreated)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        var pageResponse = new PagedResult<UserDto>()
        {
            Results = await mapper.ProjectTo<UserDto>(query).ToListAsync(),
            CurrentPage = pageIndex,
            RowCount = totalRow,
            PageSize = pageSize
        };
        return Ok(pageResponse);
    }

    [HttpPost]
    [ValidateModel]
    [Authorize(Users.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if ((await userManager.FindByNameAsync(request.UserName) != null))
        {
            return BadRequest();
        }
        if ((await userManager.FindByEmailAsync(request.Email) != null))
        {
            return BadRequest();
        }
        var user = mapper.Map<CreateUserRequest, AppUser>(request);
        var result = await userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
    }

    [HttpPut("{id}")]
    [ValidateModel]
    [Authorize(Users.Edit)]
    public async Task<IActionResult> UpdateUser(Guid id,[FromBody] UpdateUserRequest request)
    {
       var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return BadRequest();
        }
        mapper.Map(request, user);
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
    }

    [HttpPut("password-change-current-user")]
    [ValidateModel]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(User.GetUserId().ToString());
        if (user == null)
        {
            return NotFound();
        }
        var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
    }

    [HttpDelete]
    [Authorize(Permissions.Users.Delete)]
    public async Task<IActionResult> DeleteUsers([FromQuery] List<Guid> ids)
    {
        foreach (var id in ids)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            await userManager.DeleteAsync(user);
        }
        return Ok();
    }

    [HttpPost("set-password/{id}")]
    [ValidateModel]
    [Authorize(Users.Edit)]
    public async Task<IActionResult> SetPassword(Guid id, [FromBody] SetPasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return BadRequest();
        }

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.NewPassword);
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
    }

    [HttpPost("change-email/{id}")]
    [ValidateModel]
    [Authorize(Users.Edit)]
    public async Task<IActionResult> ChangeEmail(Guid id, [FromBody] ChangeEmailRequest request)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return BadRequest();
        }

        var token = await userManager.GenerateChangeEmailTokenAsync(user, request.Email);
        var result = await userManager.ChangeEmailAsync(user, request.Email, token);

        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
    }
    [HttpPut("{id}/assign-users")]
    [ValidateModel]
    [Authorize(Permissions.Users.Edit)]
    public async Task<IActionResult> AssignRolesToUser(string id, [FromBody] string[] roles)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        var currentRoles = await userManager.GetRolesAsync(user);
        var removedResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        var addedResult = await userManager.AddToRolesAsync(user, roles);
        if (!addedResult.Succeeded || !removedResult.Succeeded)
        {
            List<IdentityError> addedErrorList = addedResult.Errors.ToList();
            List<IdentityError> removedErrorList = removedResult.Errors.ToList();
            var errorList = new List<IdentityError>();
            errorList.AddRange(addedErrorList);
            errorList.AddRange(removedErrorList);

            return BadRequest(string.Join("<br/>", errorList.Select(x => x.Description)));
        }
        return Ok();
    }
}
