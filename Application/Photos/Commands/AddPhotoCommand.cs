using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Photos;
using MediatR;

namespace ClinicaWeb.Application.Photos.Commands
{
    public class AddPhotoCommand : IRequest<int>
    {
        public AddPhotoCommand(PhotoDto model)
        {
            PhotoData = model.PhotoData;
        }
        public byte[] PhotoData { get; set; }
    }
    public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand, int>
    {
        private readonly AppDbContext _appDbContext;

        public AddPhotoCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<int> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        {
            var photoToAdd = new Photo
            {
                PhotoData = request.PhotoData
            };
            _appDbContext.Photos.Add(photoToAdd);

            await _appDbContext.SaveChangesAsync();

            return photoToAdd.Id;
        }
    }
}
