using DataAccess.AutoMapper;
using DataAccess.Repositories;
using DataAccess.UnitOfWork;
using Domain.Interfaces.RepositoryInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.AspNetCore.Identity.UI.Services;
using SocialNetworkBe.Services.EmailServices;
using SocialNetworkBe.Services.OTPServices;
using SocialNetworkBe.Services.TokenServices;
using SocialNetworkBe.Services.UserServices;

namespace SocialNetworkBe.AddServicesCollection
{
    public static class AddLifeCycle
    {
        public static void ConfigureLifeCycle(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddScoped<UserService>();
            services.AddScoped<TokenService>();
            services.AddScoped<OTPService>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        }
    }
}
