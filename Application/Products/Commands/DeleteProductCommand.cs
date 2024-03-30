using ClinicaWeb.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Products.Commands
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public DeleteProductCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var productToDelete = _appDbContext.Products.Where(a => a.Id == request.Id).FirstOrDefault();

            _appDbContext.Products.Remove(productToDelete);
            _appDbContext.SaveChanges();
            return Unit.Value;
        }
    }
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator(IServiceProvider services)
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

                var productExists = db.Products.Any(d => d.Id == obj.Id);
                if (!productExists)
                {
                    context.AddFailure("Invalid product.");
                }
            });
        }
    }
}
