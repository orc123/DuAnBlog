using AutoMapper;
using DuAnBlog.Core.Domain.Identity;
using DuAnBlog.Core.Repositories;
using DuAnBlog.Core.SeedWorks;
using DuAnBlog.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DuAnBlog.Data.SeedWorks;
public class UnitOfWork : IUnitOfWork
{
    private readonly DuAnBlogContext _context;
    public UnitOfWork(DuAnBlogContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        Posts = new PostRepository(_context, mapper, userManager);
        PostCategories = new PostCategoryRepository(_context, mapper);
        Series = new SeriesRepository(_context, mapper);
    }

    public IPostRepository Posts { get; private set; }
    public IPostCategoryRepository PostCategories { get; private set; }

    public ISeriesRepository Series { get; private set; }

    public async Task<int> CompleteAsync()
    {
       return await _context.SaveChangesAsync();
    }
    public void Dispose() 
    {
        _context.Dispose();
    }
}
