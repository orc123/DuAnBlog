using AutoMapper;
using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.SeedWorks;
using DuAnBlog.Core.SeedWorks.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DuAnBlog.Api.Controllers.AdminApi;
[Route("api/admin/series")]
[ApiController]
public class SeriesController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [Authorize(Permissions.Series.Create)]
    public async Task<IActionResult> CreateSeries([FromBody] CreateUpdateSeriesRequest request)
    {
        var series = _mapper.Map<CreateUpdateSeriesRequest, Series>(request);

        _unitOfWork.Series.Add(series);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }

    [HttpPut]
    [Authorize(Permissions.Series.Edit)]
    public async Task<IActionResult> UpdateSeries(Guid id, [FromBody] CreateUpdateSeriesRequest request)
    {
        var series = await _unitOfWork.Series.GetByIdAsync(id);
        if (series == null)
        {
            return NotFound();
        }
        _mapper.Map(request, series);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }
    [Route("post-series")]
    [HttpPut()]
    [Authorize(Permissions.Series.Edit)]
    public async Task<IActionResult> AddPostSeries([FromBody] AddPostSeriesRequest request)
    {
        var isExisted = await _unitOfWork.Series.IsPostInSeries(request.SeriesId, request.PostId);
        if (isExisted)
        {
            return BadRequest($"Bài viết này đã nằm trong loạt bài.");
        }
        await _unitOfWork.Series.AddPostToSeries(request.SeriesId, request.PostId, request.SortOrder);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }

    [Route("post-series")]
    [HttpDelete()]
    [Authorize(Permissions.Series.Edit)]
    public async Task<IActionResult> DeletePostSeries([FromBody] AddPostSeriesRequest request)
    {
        var isExisted = await _unitOfWork.Series.IsPostInSeries(request.SeriesId, request.PostId);
        if (!isExisted)
        {
            return NotFound();
        }
        await _unitOfWork.Series.RemovePostToSeries(request.SeriesId, request.PostId);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }

    [Route("post-series/{seriesId}")]
    [HttpGet()]
    [Authorize(Permissions.Series.Edit)]
    public async Task<ActionResult<List<PostInListDto>>> GetPostsInSeries(Guid seriesId)
    {
        var posts = await _unitOfWork.Series.GetAllPostsInSeries(seriesId);
        return Ok(posts);
    }

    [HttpDelete]
    [Authorize(Permissions.Series.Delete)]
    public async Task<IActionResult> DeleteSeries([FromQuery] Guid[] ids)
    {
        foreach (var id in ids)
        {
            var series = await _unitOfWork.Series.GetByIdAsync(id);
            if (series == null)
            {
                return NotFound();
            }
            _unitOfWork.Series.Remove(series);
        }
        var result = await _unitOfWork.CompleteAsync();
        return result > 0 ? Ok() : BadRequest();
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Permissions.Series.View)]
    public async Task<ActionResult<SeriesDto>> GetSeriesById(Guid id)
    {
        var series = await _unitOfWork.Series.GetByIdAsync(id);
        if (series == null)
        {
            return NotFound();
        }
        return Ok(series);
    }

    [HttpGet]
    [Route("paging")]
    [Authorize(Permissions.Series.View)]
    public async Task<ActionResult<PagedResult<SeriesInListDto>>> GetSeriesPaging(string? keyword, int pageIndex, int pageSize = 10)
    {
        var result = await _unitOfWork.Series.GetAllPaging(keyword, pageIndex, pageSize);

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Permissions.Series.View)]
    public async Task<ActionResult<List<SeriesInListDto>>> GetAllSeries()
    {
        var result = await _unitOfWork.Series.GetAllAsync();
        var series = _mapper.Map<List<SeriesInListDto>>(result);
        return Ok(series);
    }
}
