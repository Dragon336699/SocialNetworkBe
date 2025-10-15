using Domain.Entities;
using DataAccess.DbContext;
using Microsoft.AspNetCore.Identity;
using DataAccess.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Domain.AddServicesCollection
{
    public static class AddServices
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration config)
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

            services.AddAuthentication(options =>
            {
                // Kích hoạt middleware xác thực
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Mặc định dùng JWT Bearer Token
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["JWT:Issuer"],
                        ValidAudience = config["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
                    };
                    // Cấu hình lấy JWT từ Cookie, ghi đề các sự kiện xảy ra trong quá trình xác thực JWT
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => // context chứa thông tin về request hiện tại
                        {
                            if (context.Request.Cookies.ContainsKey("jwt"))
                            {
                                context.Token = context.Request.Cookies["jwt"]; // Gán vào context.Token để app xác thực bằng cái này
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSignalR();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });
        }
    }
}
