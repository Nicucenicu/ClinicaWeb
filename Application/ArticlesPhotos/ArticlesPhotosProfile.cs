using AutoMapper;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Articles;

namespace ClinicaWeb.Application.ArticlesPhotos
{
    public class ArticlesPhotosProfile : Profile
    {
        public ArticlesPhotosProfile()
        {
            CreateMap<ArticlePhoto, ArticlesPhotosDto>();
        }
    }
}
