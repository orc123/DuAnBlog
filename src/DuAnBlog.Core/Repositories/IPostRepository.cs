using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.SeedWorks;

namespace DuAnBlog.Core.Repositories;
public interface IPostRepository : IRepository<Post, Guid>
{
    Task<List<Post>> GetPopularPostsAsync(int count);
}
