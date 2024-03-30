using ClinicaWeb.Persistence;
using MediatR;

namespace ClinicaWeb.Application.ArticlesPhotos.Commands
{
    public class DeleteArticlesPhotosCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
    public class DeleteArticlesPhotosCommandHandler : IRequestHandler<DeleteArticlesPhotosCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public DeleteArticlesPhotosCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Unit> Handle(DeleteArticlesPhotosCommand request, CancellationToken cancellationToken)
        {
            var articlesPhotosToDelete = _appDbContext.ArticlesPhotos.Where(a => a.Id == request.Id).FirstOrDefault();

            _appDbContext.ArticlesPhotos.Remove(articlesPhotosToDelete);
            _appDbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
