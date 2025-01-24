using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DuAnBlog.Data;
public class TeduBlogContextFactory : IDesignTimeDbContextFactory<DuAnBlogContext>
{
    public DuAnBlogContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();
        var builder = new DbContextOptionsBuilder<DuAnBlogContext>();
        builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        return new DuAnBlogContext(builder.Options);
    }
}