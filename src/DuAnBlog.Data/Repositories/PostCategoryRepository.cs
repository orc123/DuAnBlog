using AutoMapper;
using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.Repositories;
using DuAnBlog.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DuAnBlog.Data.Repositories;
public class PostCategoryRepository(DuAnBlogContext context, IMapper mapper) : RepositoryBase<PostCategory, Guid>(context), IPostCategoryRepository
{
    public async Task<PagedResult<PostCategoryDto>> GetAllPaging(string? keywork, int pageIndex = 1, int pageSize = 10)
    {
        var query = _context.PostCategories.AsQueryable();
        if (!string.IsNullOrEmpty(keywork))
        {
            query = query.Where(x => x.Name.Contains(keywork));
        }
        var totalRow = await query.CountAsync();

        query = query.OrderByDescending(x => x.DateCreated)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        return new PagedResult<PostCategoryDto>
        {
            Results = await mapper.ProjectTo<PostCategoryDto>(query).ToListAsync(),
            CurrentPage = pageIndex,
            RowCount = totalRow,
            PageSize = pageSize
        };
    }
}
