using AutoMapper;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Articles;

namespace ClinicaWeb.Application.Articles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleDto>();
        }
    }
}
