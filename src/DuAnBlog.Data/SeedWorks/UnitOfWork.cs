using AutoMapper;
using DuAnBlog.Core.Repositories;
using DuAnBlog.Core.SeedWorks;
using DuAnBlog.Data.Repositories;

namespace DuAnBlog.Data.SeedWorks;
public class UnitOfWork : IUnitOfWork
{
    private readonly DuAnBlogContext _context;
    public UnitOfWork(DuAnBlogContext context, IMapper mapper)
    {
        _context = context;
        Posts = new PostRepository(_context, mapper);
    }

    public IPostRepository Posts { get; private set; }

    public async Task<int> CompleteAsync()
    {
       return await _context.SaveChangesAsync();
    }
    public void Dispose() 
    {
        _context.Dispose();
    }
}
