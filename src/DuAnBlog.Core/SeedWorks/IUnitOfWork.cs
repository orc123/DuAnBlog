using DuAnBlog.Core.Repositories;

namespace DuAnBlog.Core.SeedWorks;
public interface IUnitOfWork
{
    IPostRepository Posts { get; }
    Task<int> CompleteAsync();
}