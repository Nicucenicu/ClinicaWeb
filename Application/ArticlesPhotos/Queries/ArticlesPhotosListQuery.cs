using ClinicaWeb.Persistence;
using MediatR;

namespace ClinicaWeb.Application.ArticlesPhotos.Queries
{
    public class ArticlesPhotosListQuery : IRequest<List<int>>
    {
        public int ArticleId { get; set; }

        public ArticlesPhotosListQuery(int articleId)
        {
            ArticleId = articleId;
        }
    }
    public class ArticlesPhotosListQueryHandler : IRequestHandler<ArticlesPhotosListQuery, List<int>>
    {
        private readonly AppDbContext _appDbContext;

        public ArticlesPhotosListQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<int>> Handle(ArticlesPhotosListQuery request, CancellationToken cancellationToken)
        {
            var photoIds = _appDbContext.ArticlesPhotos
               .Where(ap => ap.ArticleId == request.ArticleId)
               .Select(ap => ap.PhotoId)
               .ToList();

            return photoIds;
        }
    }

}
