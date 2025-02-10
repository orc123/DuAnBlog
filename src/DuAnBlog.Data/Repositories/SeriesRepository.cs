using AutoMapper;
using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.Repositories;
using DuAnBlog.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DuAnBlog.Data.Repositories;
public class SeriesRepository(DuAnBlogContext context, IMapper mapper) : RepositoryBase<Series, Guid>(context), ISeriesRepository
{
    private readonly IMapper _mapper = mapper;
    public async Task AddPostToSeries(Guid seriesId, Guid postId, int sortOrder)
    {
        var postInSeries = await _context.PostInSeries.FirstOrDefaultAsync(x => x.PostId == postId && x.SeriesId == seriesId);
        if (postInSeries == null) {
            await _context.PostInSeries.AddAsync(new PostInSeries
            {
                PostId = postId,
                SeriesId = seriesId,
                DisplayOrder = sortOrder
            });
        }
    }

    public async Task<PagedResult<SeriesInListDto>> GetAllPaging(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = _context.Series.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => x.Name.Contains(keyword));

        var totalCount = await query.CountAsync();
        query = query.OrderByDescending(x => x.DateCreated)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
        return new PagedResult<SeriesInListDto>()
        {
            Results = await _mapper.ProjectTo<SeriesInListDto>(query).ToListAsync(),
            CurrentPage = pageIndex,
            RowCount = totalCount,
            PageSize = pageSize
        };
    }

    public async Task<List<PostInListDto>> GetAllPostsInSeries(Guid seriesId)
    {
        var query = from pis in _context.PostInSeries
                    join p in _context.Posts
                        on pis.PostId equals p.Id
                    where pis.SeriesId == seriesId
                    select p;
        return await _mapper.ProjectTo<PostInListDto>(query).ToListAsync();

    }

    public async Task<bool> IsPostInSeries(Guid seriesId, Guid postId)
    {
        return await _context.PostInSeries.AnyAsync(x => x.SeriesId == seriesId && x.PostId == postId);
    }

    public async Task RemovePostToSeries(Guid seriesId, Guid postId)
    {
        var postInSeries = await _context.PostInSeries.FirstOrDefaultAsync(x => x.PostId == postId && x.SeriesId == seriesId);
        if (postInSeries != null)
        {
            _context.PostInSeries.Remove(postInSeries);
        }
    }
}
