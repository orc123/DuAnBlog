using DuAnBlog.Core.Repositories;

namespace DuAnBlog.Core.SeedWorks;
public interface IUnitOfWork
{
    IPostRepository Posts { get; }
    IPostCategoryRepository PostCategories { get; }
    ISeriesRepository Series { get; }
    Task<int> CompleteAsync();
}