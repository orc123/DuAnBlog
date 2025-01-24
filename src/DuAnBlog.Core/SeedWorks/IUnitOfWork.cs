namespace DuAnBlog.Core.SeedWorks;
public interface IUnitOfWork
{
    Task<int> CompleteAsync();
}