using AutoMapper;
using Domain.Contracts.Requests;
using Domain.Entities;

namespace DataAccess.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegistrationRequest, User>();

            CreateMap<UserRegistrationRequest, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
