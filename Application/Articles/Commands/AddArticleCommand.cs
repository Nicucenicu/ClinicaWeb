using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Articles;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Articles.Commands
{
    public class AddArticleCommand : IRequest<int>
    {
        public AddArticleCommand(ArticleDto model)
        {
            Title = model.Title;
            Subtitle = model.Subtitle;
            Content = model.Content;
            Author = model.Author;
        }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public List<byte[]> uploadedImages { get; set; }
    }
    public class AddArticleCommandHandler : IRequestHandler<AddArticleCommand, int>
    {
        private readonly AppDbContext _appDbContext;

        public AddArticleCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<int> Handle(AddArticleCommand request, CancellationToken cancellationToken)
        {
            var articleToAdd = new Article
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                Content = request.Content,
                Author = request.Author
            };
            _appDbContext.Articles.Add(articleToAdd);

            await _appDbContext.SaveChangesAsync();

            //foreach (var imageBytes in request.uploadedImages)
            //{
            //    var photo = new Photo { PhotoData = imageBytes };
            //    _appDbContext.Photos.Add(photo);
            //    await _appDbContext.SaveChangesAsync();

            //    var articlePhoto = new ArticlesPhotos { ArticleId = articleToAdd.Id, PhotoId = photo.Id };
            //    _appDbContext.ArticlesPhotos.Add(articlePhoto);
            //    await _appDbContext.SaveChangesAsync();
            //}

            return articleToAdd.Id;
        }
    }
    public class AddArticleCommandValidator : AbstractValidator<AddArticleCommand>
    {
        public AddArticleCommandValidator(IServiceProvider services)
        {
            RuleFor(d => d.Title)
                .NotEmpty();

            RuleFor(d => d.Content)
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

