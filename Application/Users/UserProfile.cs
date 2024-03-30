using AutoMapper;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Users;

namespace ClinicaWeb.Application.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
