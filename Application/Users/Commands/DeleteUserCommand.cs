using ClinicaWeb.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Users.Commands
{
    public class DeleteUserCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public DeleteUserCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userToDelete = _appDbContext.Users.Where(c => c.Id == request.Id).FirstOrDefault();

            _appDbContext.Users.Remove(userToDelete);
            _appDbContext.SaveChanges();
            return Unit.Value;
        }
    }

    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator(IServiceProvider services, IHttpContextAccessor httpContextAccessor)
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

                var submitExists = db.Users.Any(d => d.Id == obj.Id);
                if (!submitExists)
                {
                    context.AddFailure("Invalid user ");
                }

            });
        }
    }
}
