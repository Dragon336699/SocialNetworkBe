using DataAccess.AutoMapper;
using DataAccess.Repositories;
using DataAccess.UnitOfWork;
using Domain.Interfaces.BuilderInterfaces;
using Domain.Interfaces.ChatInterfaces;
using Domain.Interfaces.RepositoryInterfaces;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using SocialNetworkBe.Services.CommentServices;
using SocialNetworkBe.Services.ConversationServices;
using SocialNetworkBe.Services.ConversationUserServices;
using SocialNetworkBe.Services.EmailServices;
using SocialNetworkBe.Services.FriendRequestServices;
using SocialNetworkBe.Services.MessageService;
using SocialNetworkBe.Services.NotificationService;
using SocialNetworkBe.Services.NotificationServices.NotificationDataBuilder;
using SocialNetworkBe.Services.OTPServices;
using SocialNetworkBe.Services.PostServices;
using SocialNetworkBe.Services.RealtimeServices;
using SocialNetworkBe.Services.TokenServices;
using SocialNetworkBe.Services.UploadService;
using SocialNetworkBe.Services.UserServices;
using SocialNetworkBe.Services.GroupServices;
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
            services.AddTransient<IMessageReactionUserRepository, MessageReactionUserRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IPostReactionUserRepository, PostReactionUserRepository>();
            services.AddTransient<IFriendRequestRepository, FriendRequestRepository>();
            services.AddTransient<IUserRelationRepository, UserRelationRepository>();
            services.AddTransient<IMessageAttachmentRepository, MessageAttachmentRepository>();
            services.AddTransient<IConversationRepository, ConversationRepository>();
            services.AddTransient<IPostRepository, PostRepository>(); 
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<ICommentReactionUserRepository, CommentReactionUserRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IGroupUserRepository, GroupUserRepository>();

            services.AddTransient<IConversationUserService, ConversationUserService>();
            services.AddTransient<IConversationService, ConversationService>();        
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IFriendRequestService, FriendRequestService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IConversationUserService, ConversationUserService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IRealtimeService, RealTimeService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IGroupService, GroupService>();


            services.AddScoped<TokenService>();
            services.AddScoped<OTPService>();

            services.AddTransient<INotificationDataBuilder, NotificationDataBuilder>();

            services.AddSingleton<IUserIdProvider, CustomerUserIdProvider>();
            services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        }
    }
}
