using DataAccess.AutoMapper;
using DataAccess.Repositories;
using DataAccess.UnitOfWork;
using Domain.Interfaces.RepositoryInterfaces;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using SocialNetworkBe.Services.ConversationServices;
using SocialNetworkBe.Services.ConversationUserServices;
using SocialNetworkBe.Services.EmailServices;
using SocialNetworkBe.Services.MessageService;
using SocialNetworkBe.Services.OTPServices;
using SocialNetworkBe.Services.PostServices;
using SocialNetworkBe.Services.TokenServices;
using SocialNetworkBe.Services.UploadService;
using SocialNetworkBe.Services.UserServices;
using SocialNetworkBe.SignalR;

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
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IConversationRepository, ConversationRepository>();
            services.AddTransient<IConversationUserRepository, ConversationUserRepository>();

            services.AddTransient<IConversationRepository, ConversationRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IPostImageRepository, PostImageRepository>();

            services.AddTransient<IConversationUserService, ConversationUserService>();
            services.AddTransient<IConversationService, ConversationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IPostService, PostService>();

            services.AddTransient<IMessageAttachmentRepository, MessageAttachmentRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IConversationUserService, ConversationUserService>();
            services.AddScoped<IUploadService, UploadService>();


            services.AddScoped<TokenService>();
            services.AddScoped<OTPService>();

            services.AddSingleton<IUserIdProvider, CustomerUserIdProvider>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        }
    }
}
