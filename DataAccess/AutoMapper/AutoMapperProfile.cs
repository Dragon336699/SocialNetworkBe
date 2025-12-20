using AutoMapper;
using Domain.Contracts.Requests.Group;
using Domain.Contracts.Requests.User;
using Domain.Contracts.Responses.Comment;
using Domain.Contracts.Responses.Conversation;
using Domain.Contracts.Responses.Group;
using Domain.Contracts.Responses.Message;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Group.Types;

namespace DataAccess.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegistrationRequest, User>();
            CreateMap<User, UserDto>();
            CreateMap<Message, MessageDto>();
            CreateMap<Conversation, ConversationDto>();

            CreateMap<UserRegistrationRequest, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));

            CreateMap<CreateGroupRequest, Group>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Trim()))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic ? 1 : 0))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()) // Xử lý riêng
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.GroupUsers, opt => opt.Ignore())
                .ForMember(dest => dest.Posts, opt => opt.Ignore());

            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic == 1))
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.GroupUsers != null ? src.GroupUsers.Count(gu => gu.RoleName != GroupRole.Pending) : 0))
                .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts != null ? src.Posts.Count : 0))
                .ForMember(dest => dest.GroupUsers, opt => opt.MapFrom(src => src.GroupUsers))
                .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts));
            CreateMap<GroupUser, GroupUserDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.CommentImages, opt => opt.MapFrom(src => src.CommentImage))
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
                .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.CommentReactionUsers, opt => opt.MapFrom(src => src.CommentReactionUsers));
            CreateMap<CommentImage, CommentImageDto>();

            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dest => dest.PostImages, opt => opt.MapFrom(src => src.PostImages))
                .ForMember(dest => dest.PostReactionUsers, opt => opt.MapFrom(src => src.PostReactionUsers));
            CreateMap<PostImage, PostImageDto>();
            CreateMap<PostImage, PostImageDto>();
            CreateMap<PostReactionUser, PostReactionUserDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<UpdateGroupRequest, Group>()
                .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Name)))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Description)))
                .ForMember(dest => dest.IsPublic, opt => opt.Condition(src => src.IsPublic.HasValue))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic.HasValue ? (src.IsPublic.Value ? 1 : 0) : 0))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
