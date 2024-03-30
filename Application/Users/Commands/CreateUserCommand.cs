using ClinicaWeb.Application.EmailsInfrastructure;
using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace ClinicaWeb.Application.Users.Commands
{
    public class CreateUserCommand : IRequest<int>
    {
        public CreateUserCommand(CreateUserDto model)
        {
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email.Trim();
            UserName = model.UserName.Trim();
            Password = model.Password;
            ConfirmPassword = model.ConfirmPassword;
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEmailService _emailService;
        private readonly IServiceProvider _serviceProvider;
        public string HostName = "https://localhost/7234";
        public CreateUserCommandHandler(AppDbContext appDbContext, IEmailService emailService, IServiceProvider serviceProvider)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _appDbContext.Users.AnyAsync(user => user.Email.ToLower().Equals(request.Email.ToLower()));

            var userToAdd = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsEmailVerified = true,
                VerificationToken = CreateRandomToken(),
            };

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userToAdd.PasswordHash = passwordHash;
            userToAdd.PasswordSalt = passwordSalt;


            _appDbContext.Users.Add(userToAdd);
            await _appDbContext.SaveChangesAsync();

            SendRegistrationEmail(userToAdd.Email, userToAdd.FirstName, request.Password);

            return userToAdd.Id;
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private void SendRegistrationEmail(string userEmail, string firstName, string password)
        {
            var emailTemplate = _appDbContext.EmailTemplates.FirstOrDefault(x => x.Name == "UserCreateEmail");
            byte[] data = Convert.FromBase64String(emailTemplate.Body);
            string template = System.Text.Encoding.UTF8.GetString(data);

            template = template.Replace("{BaseUrl}", HostName)
                .Replace("{Password}", password)
                .Replace("{FirstName}", firstName);

            _emailService.QuickSendAsync(
                subject: "Bun venit in ALMA clinic!",
                body: template,
                to: userEmail);
        }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IServiceProvider services, IHttpContextAccessor httpContextAccessor)
        {
            RuleFor(d => d.FirstName)
               .NotNull()
               .NotEmpty();

            RuleFor(d => d.LastName)
               .NotNull()
               .NotEmpty();

            RuleFor(d => d).Custom((obj, context) =>
            {
                if (string.IsNullOrEmpty(obj.FirstName) || string.IsNullOrEmpty(obj.LastName))
                {
                    context.AddFailure("First and Last Names are required!");
                    return;
                }

                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                if (db == null)
                {
                    context.AddFailure("Internal problem - needs admin attention.");
                    return;
                }

                var emailExists = db.Users.Any(d => d.Email == obj.Email.ToLower());

                if (emailExists)
                {
                    context.AddFailure("This email is already associated with an account!");
                }
            });
        }
    }
}
