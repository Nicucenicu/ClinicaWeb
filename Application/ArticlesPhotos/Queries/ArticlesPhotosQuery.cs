using AutoMapper;
using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Articles;
using MediatR;

namespace ClinicaWeb.Application.ArticlesPhotos.Queries
{
    public class ArticlesPhotosQuery : IRequest<ArticlesPhotosDto>
    {
        public ArticlesPhotosQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
    public class ArticlesPhotosQueryHandler : IRequestHandler<ArticlesPhotosQuery, ArticlesPhotosDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ArticlesPhotosQueryHandler(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<ArticlesPhotosDto> Handle(ArticlesPhotosQuery request, CancellationToken cancellationToken)
        {
            var articlesPhotos = _appDbContext.ArticlesPhotos.Where(a => a.Id == request.Id).FirstOrDefault();

            return _mapper.Map<ArticlesPhotosDto>(articlesPhotos);
        }
    }
}
