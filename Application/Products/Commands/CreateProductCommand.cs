using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Products;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<int>
    {
        public CreateProductCommand(CreateProductDto model)
        {
            Name = model.Name;
            ShortDescription = model.ShortDescription;
            Description = model.Description;
            CategoryId = model.CategoryId;
            Price = model.Price;
        }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
    }
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly AppDbContext _appDbContext;

        public CreateProductCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var articleToCreate = new Product
            {
                Name = request.Name,
                ShortDescription = request.ShortDescription,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Price = request.Price,
            };
            _appDbContext.Products.Add(articleToCreate);
            await _appDbContext.SaveChangesAsync();

            return articleToCreate.Id;
        }
    }
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(IServiceProvider services)
        {
            RuleFor(d => d.Name)
                .NotEmpty();

            RuleFor(d => d.Description)
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
            });
        }
    }
}

