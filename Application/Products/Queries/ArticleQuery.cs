using AutoMapper;
using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Articles;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace ClinicaWeb.Application.Articles.Queries
{
    public class ArticleQuery : IRequest<ArticleDto>
    {
        public ArticleQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class ArticleQueryHandler : IRequestHandler<ArticleQuery, ArticleDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ArticleQueryHandler(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<ArticleDto> Handle(ArticleQuery request, CancellationToken cancellationToken)
        {
            var article = _appDbContext.Articles.Where(a => a.Id == request.Id).FirstOrDefault();

            return _mapper.Map<ArticleDto>(article);
        }
    }
    public class ArticleQueryValidator : AbstractValidator<ArticleQuery>
    {
        public ArticleQueryValidator(IServiceProvider services)
        {

            //Custom validations with database
            RuleFor(d => d).Custom((obj, context) =>
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                if (db == null)
                {
                    context.AddFailure("Internal problem - needs admin attention.");
                    return;
                }

                var articleExists = db.Articles.Any(d => d.Id == obj.Id);
                if (!articleExists)
                {
                    context.AddFailure("Invalid article.");
                }

            });
        }
    }
}
