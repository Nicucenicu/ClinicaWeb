using AutoMapper;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Photos;

namespace ClinicaWeb.Application.Photos
{
    public class PhotoProfile : Profile
    {
        public PhotoProfile()
        {
            CreateMap<Photo, PhotoDto>();
        }
    }
}
