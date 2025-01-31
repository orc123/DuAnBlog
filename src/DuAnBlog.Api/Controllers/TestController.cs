﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DuAnBlog.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello from TestController");
    }
}
