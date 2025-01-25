using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.SeedWorks;

namespace DuAnBlog.Core.Repositories;
public interface IPostRepository : IRepository<Post, Guid>
{
    Task<List<Post>> GetPopularPostsAsync(int count);
    Task<PagedResult<PostInListDto>> GetPostsPagingAsync(string? keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10);
}
