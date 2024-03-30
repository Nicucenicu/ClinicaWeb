using ClinicaWeb.Application.Users.Commands;
using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace ClinicaWeb.Application.Users.Commands
{
    public class EditMyDetailsCommand : IRequest<Unit>
    {
        public EditMyDetailsCommand(UserDto model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email.Trim();
            UserName = model.UserName.Trim();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
public class EditMyDetailsCommandHandler : IRequestHandler<EditMyDetailsCommand, Unit>
{
    private readonly AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public EditMyDetailsCommandHandler(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _appDbContext = appDbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Unit> Handle(EditMyDetailsCommand request, CancellationToken cancellationToken)
    {
        var userToEdit = _appDbContext.Users.Where(c => c.Id == request.Id).FirstOrDefault();
        var claimUserid = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        int.TryParse(claimUserid, out int claimUseridAsInt);
        var user = _appDbContext.Users.Where(u => u.Id == claimUseridAsInt).FirstOrDefault();

        if (userToEdit.Id == user.Id)
        {
            if (!string.IsNullOrEmpty(request.FirstName))
            {
                userToEdit.FirstName = request.FirstName;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                userToEdit.LastName = request.LastName;
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                userToEdit.Email = request.Email;
            }
        }

        _appDbContext.SaveChanges();

        return Unit.Value;
    }
}
public class EditMyDetailsCommandValidator : AbstractValidator<EditMyDetailsCommand>
{
    public EditMyDetailsCommandValidator(IServiceProvider services)
    {
        RuleFor(d => d.FirstName)
           .NotNull()
           .NotEmpty();

        RuleFor(d => d.LastName)
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

            var doublicatedDataUsers = db.Users
                .Where(d => d.Email == obj.Email.ToLower() || d.Id == obj.Id)
                .ToList();

            if (doublicatedDataUsers.FirstOrDefault(u => u.Id == obj.Id) == null)
            {
                context.AddFailure("Invalid user ");
            }

            var emailExists = doublicatedDataUsers.Any(d => d.Email.ToLower() == obj.Email?.ToLower() && d.Id != obj.Id);

            if (emailExists)
            {
                context.AddFailure("This email is already associated with an account!");
            }
        });
    }
}

