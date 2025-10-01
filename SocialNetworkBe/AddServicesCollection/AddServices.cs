using Domain.Entities;
using DataAccess.DbContext;
using Microsoft.AspNetCore.Identity;
using DataAccess.AutoMapper;

namespace Domain.AddServicesCollection
{
    public static class AddServices
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddRoles<Role>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<SocialNetworkDbContext>();
        }
    }
}
