using ClinicaWeb.Persistence;
using MediatR;

namespace ClinicaWeb.Application.Photos.Commands
{
    public class DeletePhotoCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
    public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public DeletePhotoCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Unit> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var photoToDelete = _appDbContext.Photos.Where(p => p.Id == request.Id).FirstOrDefault();

            _appDbContext.Photos.Remove(photoToDelete);
            _appDbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
