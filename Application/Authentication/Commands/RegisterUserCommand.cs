using ClinicaWeb.Application.EmailsInfrastructure;
using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Users;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace ClinicaWeb.Application.Authentication.Commands
{
    public class RegisterUserCommand : IRequest<UserRegistrationResponseDto>
    {
        public RegisterUserCommand(UserRegisterDto model)
        {
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
            UserName = model.UserName;
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

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserRegistrationResponseDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEmailService _emailService;
        public string HostName = "https://localhost/7234/verify";
        private readonly object lockObject = new object();
        private ManualResetEvent emailSentEvent = new ManualResetEvent(false);

        public RegisterUserCommandHandler(AppDbContext appDbContext, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
        }

        public async Task<UserRegistrationResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _appDbContext.Users.AnyAsync(user => user.Email.ToLower().Equals(request.Email.ToLower()));
            var userToAdd = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                VerificationCode = Guid.NewGuid(),
                VerificationToken = CreateRandomToken(),
                IsEmailVerified = false,
            };

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userToAdd.PasswordHash = passwordHash;
            userToAdd.PasswordSalt = passwordSalt;


            _appDbContext.Users.Add(userToAdd);
            await _appDbContext.SaveChangesAsync();

            SendRegistrationEmail(userToAdd.Email, userToAdd.FirstName, userToAdd.VerificationCode.Value, userToAdd.VerificationToken);

            return new UserRegistrationResponseDto
            {
                UserId = userToAdd.Id,
                NeedsApproval = false
            };
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void SendRegistrationEmail(string userEmail, string firstName, Guid verificationCode, string token)
        {
            var emailTemplate = _appDbContext.EmailTemplates.FirstOrDefault(x => x.Name == "UserVerificationEmail");
            byte[] data = Convert.FromBase64String(emailTemplate.Body);
            string template = System.Text.Encoding.UTF8.GetString(data);

            template = template.Replace("{BaseUrl}", HostName)
                .Replace("{VerificationCode}", verificationCode.ToString())
                .Replace("{VerificationLink}", HostName + $"/{token}")
                .Replace("{FirstName}", firstName);

            _emailService.QuickSendAsync(
                subject: "Bun venit in Academy Center!",
                body: template,
                to: userEmail);
        }     

        private void SendRegistrationAproveEmail(string userEmail, string userName)
        {
            var emailTemplate = _appDbContext.EmailTemplates.FirstOrDefault(x => x.Name == "UserAproveCommonEmail");
            byte[] data = Convert.FromBase64String(emailTemplate.Body);
            string template = System.Text.Encoding.UTF8.GetString(data);

            template = template.Replace("{UserName}", userName);

            _emailService.QuickSendAsync(
                subject: "Actualizare privind cererea dvs.",
                body: template,
                to: userEmail);

            emailSentEvent.Set();
        }        
    }

    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(IServiceProvider services)
        {
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

                var emailExists = db.Users.Any(d => d.Email.ToLower() == obj.Email.ToLower());

                if (emailExists)
                {
                    context.AddFailure("This email is already associated with an account!");
                }
            });
        }
    }
}
