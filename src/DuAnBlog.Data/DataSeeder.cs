using DuAnBlog.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace DuAnBlog.Data;
public class DataSeeder
{
    public async Task SeedAsync(DuAnBlogContext context)
    {
        var passwordHasher = new PasswordHasher<AppUser>();

        var rootAdminRoleId = Guid.NewGuid();
        if (!context.Roles.Any())
        {
            await context.Roles.AddAsync(new AppRole()
            {
                Id = rootAdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                DisplayName = "Quản trị viên"
            });
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var userId = Guid.NewGuid();
            var user = new AppUser()
            {
                Id = userId,
                FirstName = "Đông",
                LastName = "Phạm",
                Email = "admin@duan.com.vn",
                NormalizedEmail = "ADMIN@duan.COM.VN",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now
            };
            user.PasswordHash = passwordHasher.HashPassword(user, "ABCDEF@2fedcba");
            await context.Users.AddAsync(user);

            await context.UserRoles.AddAsync(new IdentityUserRole<Guid>()
            {
                RoleId = rootAdminRoleId,
                UserId = userId,
            });
            await context.SaveChangesAsync();
        }
    }
}
