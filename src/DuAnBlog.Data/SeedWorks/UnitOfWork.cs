using DuAnBlog.Core.SeedWorks;

namespace DuAnBlog.Data.SeedWorks;
public class UnitOfWork : IUnitOfWork
{
    private readonly DuAnBlogContext _context;
    public UnitOfWork(DuAnBlogContext context)
    {
        _context = context;
    }

    public async Task<int> CompleteAsync()
    {
       return await _context.SaveChangesAsync();
    }
    public void Dispose() 
    {
        _context.Dispose();
    }
}
