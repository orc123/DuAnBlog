namespace DuAnBlog.Core.Models.System;
public class PermissionDto
{
    public string RoleId { get; set; }
    public List<RoleClaimsDto> RoleClaims { get; set; }
}
