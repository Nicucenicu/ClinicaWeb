using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Articles;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Articles.Commands
{
    public class EditProductCommand : IRequest<Unit>
    {
        public EditProductCommand(ArticleDto model)
        {
            Id = model.Id;
            Title = model.Title;
            Subtitle = model.Subtitle;
            Content = model.Content;
            Author = model.Author;
            Photoids = model.PhotoIds;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public List<int> Photoids = new List<int>();
    }
    public class EditArticleCommandHandler : IRequestHandler<EditProductCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public EditArticleCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Unit> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            var articleToEdit = _appDbContext.Articles.Where(a => a.Id == request.Id).FirstOrDefault();

            if (!string.IsNullOrEmpty(request.Title))
            {
                articleToEdit.Title = request.Title;
            }

            if (!string.IsNullOrEmpty(request.Subtitle))
            {
                articleToEdit.Subtitle = request.Subtitle;
            }

            if (!string.IsNullOrEmpty(request.Content))
            {
                articleToEdit.Content = request.Content;
            }

            if (!string.IsNullOrEmpty(request.Author))
            {
                articleToEdit.Author = request.Author;
            }

            _appDbContext.SaveChanges();

            return Unit.Value;
        }
    }
    public class EditArticleCommandValidator : AbstractValidator<EditProductCommand>
    {
        public EditArticleCommandValidator(IServiceProvider services)
        {
            RuleFor(d => d.Title)
               .NotNull()
               .NotEmpty();

            RuleFor(d => d.Content)
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

                var articleExists = db.Articles.Any(d => d.Id == obj.Id);
                if (!articleExists)
                {
                    context.AddFailure("Invalid article.");
                }
            });
        }
    }
}
