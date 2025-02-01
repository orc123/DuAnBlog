﻿using Microsoft.AspNetCore.Authorization;

namespace DuAnBlog.Api.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; private set; } = permission;
}
