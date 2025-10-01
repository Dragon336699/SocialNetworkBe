using DataAccess.AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using SocialNetworkBe.Services.EmailService;
using SocialNetworkBe.Services.UserService;

namespace SocialNetworkBe.AddServicesCollection
{
    public static class AddLifeCycle
    {
        public static void ConfigureLifeCycle(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<UserService>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        }
    }
}
