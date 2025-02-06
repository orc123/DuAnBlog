using DuAnBlog.Core.Repositories;

namespace DuAnBlog.Core.SeedWorks;
public interface IUnitOfWork
{
    IPostRepository Posts { get; }
    IPostCategoryRepository PostCategories { get; }
    Task<int> CompleteAsync();
}