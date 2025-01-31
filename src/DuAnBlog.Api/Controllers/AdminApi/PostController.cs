using AutoMapper;
using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuAnBlog.Api.Controllers.AdminApi;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize]
public class PostController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreateUpdatePostRequest request)
    {
        var post = _mapper.Map<CreateUpdatePostRequest, Post>(request);
        _unitOfWork.Posts.Add(post);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok(post) : BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] CreateUpdatePostRequest request)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        _mapper.Map(request, post);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok(post) : BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        _unitOfWork.Posts.Remove(post);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }

    [HttpGet()]
    [Route("id")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpGet()]
    [Route("paging")]
    public async Task<IActionResult> GetPostsPaging(string? keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
    {
        var posts = await _unitOfWork.Posts.GetPostsPagingAsync(keyword, categoryId, pageIndex, pageSize);
        return Ok(posts);
    }
}
