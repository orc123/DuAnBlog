using AutoMapper;
using DuAnBlog.Api.Filters;
using DuAnBlog.Core.Domain.Identity;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Auth;
using DuAnBlog.Core.SeedWorks.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DuAnBlog.Api.Controllers.AdminApi;
[Route("api/admin/[controller]")]
[ApiController]
[Authorize]
public class RoleController(RoleManager<AppRole> roleManager, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ValidateModel]
    [Authorize(Permissions.Roles.Create)]
    public async Task<IActionResult> CreateRole([FromBody] CreateUpdateRoleRequest request)
    {
        var role = await roleManager.CreateAsync(new AppRole
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DisplayName = request.DisplayName
        });
        if (role.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
    [HttpPut("{id}")]
    [ValidateModel]
    [Authorize(Permissions.Roles.Edit)]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] CreateUpdateRoleRequest request)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return NotFound();
        }
        role.Name = request.Name;
        role.DisplayName = request.DisplayName;

        await roleManager.UpdateAsync(role);
        return Ok();
    }

    [HttpDelete]
    [Authorize(Permissions.Roles.Delete)]
    public async Task<IActionResult> DeleteRoles([FromQuery] List<Guid> ids)
    {
        foreach (var id in ids)
        {
            var role = await roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }
            await roleManager.DeleteAsync(role);
        }
        return Ok();
    }

    [HttpGet("{id}")]
    [Authorize(Permissions.Roles.View)]
    public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<RoleDto>(role));
    }

    [HttpGet("paging")]
    [Authorize(Permissions.Roles.View)]
    public async Task<ActionResult<PagedResult<RoleDto>>> GetRolesPaging(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = roleManager.Roles;
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) || x.DisplayName.Contains(keyword));
        }
        var totalRecords = await query.CountAsync();
        query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        var data = await mapper.ProjectTo<RoleDto>(query).ToListAsync();
        return Ok(new PagedResult<RoleDto>
        {
            Results = data,
            RowCount = totalRecords,
            CurrentPage = pageIndex,
            PageSize = pageSize
        });
    }


    [HttpGet("all")]
    [Authorize(Permissions.Roles.View)]
    public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
    {
        var roles = await mapper.ProjectTo<RoleDto>(roleManager.Roles).ToListAsync();
        return Ok(mapper.Map<List<RoleDto>>(roles));
    }
}
