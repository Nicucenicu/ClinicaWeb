using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Articles;
using MediatR;

namespace ClinicaWeb.Application.ArticlesPhotos.Commands
{
    public class AddArticlesPhotosCommand : IRequest<int>
    {
        public AddArticlesPhotosCommand(ArticlesPhotosDto model)
        {
            ArticleId = model.ArticleId;
            PhotoId = model.PhotoId;
        }
        public int ArticleId { get; set; }
        public int PhotoId { get; set; }
    }
    public class AddArticlesPhotosCommandHandler : IRequestHandler<AddArticlesPhotosCommand, int>
    {
        private readonly AppDbContext _appDbContext;

        public AddArticlesPhotosCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<int> Handle(AddArticlesPhotosCommand request, CancellationToken cancellationToken)
        {
            var articlesPhotosToAdd = new ArticlePhoto
            {
                ArticleId = request.ArticleId,
                PhotoId = request.PhotoId
            };
            _appDbContext.ArticlesPhotos.Add(articlesPhotosToAdd);

            await _appDbContext.SaveChangesAsync();

            return articlesPhotosToAdd.Id;
        }
    }
}