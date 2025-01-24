using DuAnBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace DuAnBlog.Api;

public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using(var context = scope.ServiceProvider.GetRequiredService<DuAnBlogContext>())
            {
                context.Database.Migrate();
                new DataSeeder().SeedAsync(context).Wait();
            }
        }
        return app;
    }
}