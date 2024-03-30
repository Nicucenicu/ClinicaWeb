using ClinicaWeb.Shared.Dtos.Pagination;

namespace ClinicaWeb.Shared.Dtos.Pagination.Service
{
    public interface IPaginationService
    {
        Task<PaginationResult<DestinationT>> PaginateEnumerableAsync<TSource, DestinationT>(IEnumerable<TSource> source, PaginationParameter request);

        Task<PaginationResult<DestinationT>> PaginateAsync<TSource, DestinationT>(IQueryable<TSource> source, PaginationParameter request);
    }
}
