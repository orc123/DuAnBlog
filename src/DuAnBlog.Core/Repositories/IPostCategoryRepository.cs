using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.SeedWorks;

namespace DuAnBlog.Core.Repositories;
public interface IPostCategoryRepository : IRepository<PostCategory, Guid>
{
    Task<PagedResult<PostCategoryDto>> GetAllPaging(string? keywork, int pageIndex = 1, int pageSize = 10);
}
