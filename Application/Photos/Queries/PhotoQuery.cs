using AutoMapper;
using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Photos;
using MediatR;

namespace ClinicaWeb.Application.Photos.Queries
{
    public class PhotoQuery : IRequest<PhotoDto>
    {
        public PhotoQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
    public class PhotoQueryHandler : IRequestHandler<PhotoQuery, PhotoDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public PhotoQueryHandler(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<PhotoDto> Handle(PhotoQuery request, CancellationToken cancellationToken)
        {
            var photo = _appDbContext.Photos.Where(p => p.Id == request.Id).FirstOrDefault();

            return _mapper.Map<PhotoDto>(photo);
        }
    }
}
