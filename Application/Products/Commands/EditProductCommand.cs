using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Products;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Products.Commands
{
    public class EditProductCommand : IRequest<Unit>
    {
        public EditProductCommand(ProductDto model)
        {
            Id = model.Id;
            Name = model.Name;
            ShortDescription = model.ShortDescription;
            Description = model.Description;
            CategoryId = model.CategoryId;
            Price = model.Price;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
    }
    public class EditProductCommandHandler : IRequestHandler<EditProductCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public EditProductCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Unit> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            var productToEdit = await _appDbContext.Products.Where(a => a.Id == request.Id).FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(request.Name))
            {
                productToEdit.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.ShortDescription))
            {
                productToEdit.ShortDescription = request.ShortDescription;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                productToEdit.Description = request.Description;
            }

            if (request.CategoryId>0)
            {
                productToEdit.CategoryId = request.CategoryId;
            }

            if (request.Price > 0)
            {
                productToEdit.Price = request.Price;
            }

            _appDbContext.SaveChanges();

            return Unit.Value;
        }
    }
    public class EditProductCommandValidator : AbstractValidator<EditProductCommand>
    {
        public EditProductCommandValidator(IServiceProvider services)
        {
            RuleFor(d => d.Name)
               .NotNull()
               .NotEmpty();

            RuleFor(d => d.Description)
               .NotNull()
               .NotEmpty();

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
