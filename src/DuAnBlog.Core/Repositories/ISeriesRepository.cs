using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Models;
using DuAnBlog.Core.Models.Content;
using DuAnBlog.Core.SeedWorks;

namespace DuAnBlog.Core.Repositories;
public interface ISeriesRepository : IRepository<Series, Guid>
{
    Task<PagedResult<SeriesInListDto>> GetAllPaging(string? keyword, int page = 1, int pageSize = 10);
    Task AddPostToSeries(Guid seriesId, Guid postId, int sortOrder);
    Task RemovePostToSeries(Guid seriesId, Guid postId);
    Task<List<PostInListDto>> GetAllPostsInSeries(Guid seriesId);
    Task<bool> IsPostInSeries(Guid seriesId, Guid postId);
}
