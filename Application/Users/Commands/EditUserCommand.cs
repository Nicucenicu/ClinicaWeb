using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Users;
using ClinicaWeb.Shared.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace ClinicaWeb.Application.Users.Commands
{
    public class EditUserCommand : IRequest<Unit>
    {
        public EditUserCommand(UserDto model)
        {
            Id = model.Id;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email.Trim();
            UserName = model.UserName.Trim();
            //Role = model.Role;
            //PhotoId = model.PhotoId;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        //public string NewPassword { get; set; }

        //public RoleEnum Role { get; set; }
        //public int PhotoId { get; set; }
    }

    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditUserCommandHandler(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<Unit> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var claimUserid = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            int.TryParse(claimUserid, out int claimUseridAsInt);
            var user = _appDbContext.Users.Where(u => u.Id == claimUseridAsInt).FirstOrDefault();

            if (user.Role == Role.Administrator)
            {

                var userToEdit = _appDbContext.Users.Where(c => c.Id == request.Id).FirstOrDefault();

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

                //if (!string.IsNullOrEmpty(request.NewPassword))
                //{
                //    CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

                //    userToEdit.PasswordHash = passwordHash;
                //    userToEdit.PasswordSalt = passwordSalt;
                //}

                //if (request.Role != null)
                //{
                //    userToEdit.Role = request.Role;
                //}

                //if (request.PhotoId != null)
                //{
                //    userToEdit.PhotoId = request.PhotoId;
                //}

            }
            _appDbContext.SaveChanges();

            return Unit.Value;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }

    public class EditUserCommandValidator : AbstractValidator<EditUserCommand>
    {
        public EditUserCommandValidator(IServiceProvider services)
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
}
