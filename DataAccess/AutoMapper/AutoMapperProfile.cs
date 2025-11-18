using AutoMapper;
using Domain.Contracts.Requests.User;
using Domain.Contracts.Responses.Comment;
using Domain.Contracts.Responses.Conversation;
using Domain.Contracts.Responses.Message;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
using Domain.Entities;

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

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.CommentImages, opt => opt.MapFrom(src => src.CommentImage))
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
                .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.CommentReactionUsers, opt => opt.MapFrom(src => src.CommentReactionUsers));
            CreateMap<CommentImage, CommentImageDto>();

            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.PostImages, opt => opt.MapFrom(src => src.PostImages))
                .ForMember(dest => dest.PostReactionUsers, opt => opt.MapFrom(src => src.PostReactionUsers));
            CreateMap<PostImage, PostImageDto>();
            CreateMap<PostReactionUser, PostReactionUserDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        }
    }
}
