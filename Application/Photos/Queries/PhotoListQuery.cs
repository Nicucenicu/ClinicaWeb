using AutoMapper;
using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Photos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicaWeb.Application.Photos.Queries
{
    public class PhotoListQuery : IRequest<List<PhotoDto>>
    {
        public PhotoListQuery()
        {

        }
    }

    public class PhotoListQueryHandler : IRequestHandler<PhotoListQuery, List<PhotoDto>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public PhotoListQueryHandler(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<List<PhotoDto>> Handle(PhotoListQuery request, CancellationToken cancellationToken)
        {
            var photos = await _appDbContext.Photos.ToListAsync();
            return _mapper.Map<List<PhotoDto>>(photos);
        }
    }
}

