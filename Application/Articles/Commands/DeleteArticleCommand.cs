using ClinicaWeb.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Articles.Commands
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteArticleCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public DeleteArticleCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var articleToDelete = _appDbContext.Articles.Where(a => a.Id == request.Id).FirstOrDefault();

            _appDbContext.Articles.Remove(articleToDelete);
            _appDbContext.SaveChanges();
            return Unit.Value;
        }
    }
    public class DeleteArticleCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteArticleCommandValidator(IServiceProvider services)
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
