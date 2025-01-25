using AutoMapper;
using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.Repositories;
using DuAnBlog.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DuAnBlog.Data.Repositories;
public class PostRepository(DuAnBlogContext context, IMapper mapper) : RepositoryBase<Post, Guid>(context), IPostRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<List<Post>> GetPopularPostsAsync(int count)
    {
        return await _context.Posts.OrderByDescending(x => x.ViewCount).Take(count).ToListAsync();
    }

    public async Task<PagedResult<PostInListDto>> GetPostsPagingAsync(string? keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
    {
         var query = _context.Posts.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword));
        }
        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId);
        }
        var totalCount = await query.CountAsync();
        query = query.OrderByDescending(x => x.DateCreated)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
        return new PagedResult<PostInListDto>()
        {
            Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
            CurrentPage = pageIndex,
            RowCount = totalCount,
            PageSize = pageSize
        };
    }
}
