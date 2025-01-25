using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Repositories;
using DuAnBlog.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DuAnBlog.Data.Repositories;
public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
{
    public PostRepository(DuAnBlogContext context) : base(context)
    {
    }

    public async Task<List<Post>> GetPopularPostsAsync(int count)
    {
        return await _context.Posts.OrderByDescending(x => x.ViewCount).Take(count).ToListAsync();
    }
}
