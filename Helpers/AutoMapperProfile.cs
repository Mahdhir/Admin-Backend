using AutoMapper;
using Project.Dtos;
using Project.Entities;
using WebApi.Dtos;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Admin, AdminDto>();
            CreateMap<AdminDto, Admin>();
        }
    }
}