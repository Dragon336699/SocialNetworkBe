using AutoMapper;
using Domain.Contracts.Requests.User;
using Domain.Contracts.Responses.Message;
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

            CreateMap<UserRegistrationRequest, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
