using AutoMapper;
using ClinicaWeb.Shared.Dtos.Pagination;
using ClinicaWeb.Shared.Dtos.Pagination.Service;
using Microsoft.EntityFrameworkCore;

namespace ClinicaWeb.Application.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly IMapper _mapper;
        public PaginationService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<PaginationResult<DestinationT>> PaginateEnumerableAsync<TSource, DestinationT>(IEnumerable<TSource> source, PaginationParameter request)
        {
            var response = new PaginationResult<DestinationT>();

            response.TotalItems = source.Count();
            var items = source
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize);

            response.Items = _mapper.Map<IEnumerable<DestinationT>>(items);
            return response;
        }

        public async Task<PaginationResult<DestinationT>> PaginateAsync<TSource, DestinationT>(IQueryable<TSource> source, PaginationParameter request)
        {
            var response = new PaginationResult<DestinationT>();

            response.TotalItems = source.Count();
            var items = await source
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            response.Items = _mapper.Map<IEnumerable<DestinationT>>(items);
            return response;
        }
    }
}
