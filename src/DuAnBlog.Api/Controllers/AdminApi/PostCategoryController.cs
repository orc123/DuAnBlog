using AutoMapper;
using DuAnBlog.Api.Filters;
using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DuAnBlog.Core.SeedWorks.Contants.Permissions;

namespace DuAnBlog.Api.Controllers.AdminApi;
[Route("api/admin/post-category")]
[ApiController]
public class PostCategoryController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [ValidateModel]
    [Authorize(PostCategories.Create)]
    public async Task<IActionResult> CreatePostCategory([FromBody] CreateUpdatePostCategoryRequest request)
    {
        var postCategory = _mapper.Map<CreateUpdatePostCategoryRequest, PostCategory>(request);

        _unitOfWork.PostCategories.Add(postCategory);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }

    [HttpPut("{id}")]
    [ValidateModel]
    [Authorize(PostCategories.Edit)]
    public async Task<IActionResult> UpdatePostCategory(Guid id, [FromBody] CreateUpdatePostCategoryRequest request)
    {
        var postCategory = await _unitOfWork.PostCategories.GetByIdAsync(id);
        if (postCategory == null)
        {
            return NotFound();
        }

        _mapper.Map(request, postCategory);
        await _unitOfWork.CompleteAsync();

        return Ok();
    }

    [HttpDelete]
    [Authorize(PostCategories.Delete)]
    public async Task<IActionResult> DeletePostCategory([FromQuery] Guid[] ids)
    {
        foreach (var id in ids)
        {
            var postCategory = await _unitOfWork.PostCategories.GetByIdAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            _unitOfWork.PostCategories.Remove(postCategory);
        }
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }

    [HttpGet("{id}")]
    [Authorize(PostCategories.View)]
    public async Task<ActionResult<PostCategoryDto>> GetPostCategoryById(Guid id)
    {
        var postCategory = await _unitOfWork.PostCategories.GetByIdAsync(id);
        if (postCategory == null)
        {
            return NotFound();
        }

        var categoryDto = _mapper.Map<PostCategoryDto>(postCategory);
        return Ok(categoryDto);
    }

    [HttpGet("paging")]
    [Authorize(PostCategories.View)]
    public async Task<ActionResult<PagedResult<PostCategoryDto>>> GetPostCategoriesByPaging(string? keywork, int pageIndex = 1, int pageSize = 10) => Ok(await _unitOfWork.PostCategories.GetAllPaging(keywork, pageIndex, pageSize));

    [HttpGet()]
    [Authorize(PostCategories.View)]
    public async Task<ActionResult<List<PostCategoryDto>>> GetPostCategories()
    {
        var query = await _unitOfWork.PostCategories.GetAllAsync();
        var model = _mapper.Map<List<PostCategoryDto>>(query);
        return Ok(model);
    }
}
