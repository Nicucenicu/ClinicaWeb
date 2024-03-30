using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Articles;
using ClinicaWeb.Shared.Dtos.Pagination;
using ClinicaWeb.Shared.Dtos.Pagination.Service;
using MediatR;

namespace ClinicaWeb.Application.Articles.Queries
{
    public class ArticlesListQuery : IRequest<PaginationResult<ArticleDto>>
    {
        public PaginationParameter PaginationParameter { get; set; }

        public ArticlesListQuery(PaginationParameter paginationParameter)
        {
            PaginationParameter = paginationParameter;
        }
    }

    public class ArticleListQueryHandler : IRequestHandler<ArticlesListQuery, PaginationResult<ArticleDto>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IPaginationService _paginationService;
        public ArticleListQueryHandler(AppDbContext appDbContext, IPaginationService paginationService)
        {
            _appDbContext = appDbContext;
            _paginationService = paginationService;
        }

        public async Task<PaginationResult<ArticleDto>> Handle(ArticlesListQuery request, CancellationToken cancellationToken)
        {
            var articles = _appDbContext.Articles.AsQueryable();

            return await _paginationService.PaginateAsync<Article, ArticleDto>(articles, request.PaginationParameter);
        }
    }
}
