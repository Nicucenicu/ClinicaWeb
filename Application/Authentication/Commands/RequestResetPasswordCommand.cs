using ClinicaWeb.Application.EmailsInfrastructure;
using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Users;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Authentication.Commands
{
    public class RequestResetPasswordCommand : IRequest<Unit>
    {
        public RequestResetPasswordCommand(RequestResetPasswordDto email)
        {
            Email = email.Email;
        }
        public string Email { get; set; }
    }

    public class RequestResetPasswordCommandHandler : IRequestHandler<RequestResetPasswordCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEmailService _emailService;
        public string HostName = "https://localhost/7234/reset-password";

        public RequestResetPasswordCommandHandler(AppDbContext appDbContext, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
        }
        public async Task<Unit> Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = _appDbContext.Users.Where(x => x.Email == request.Email).FirstOrDefault();

            if (user != null)
            {
                var resets = new ResetPasswordField
                {
                    PasswordGuid = Guid.NewGuid(),
                    UserId = user.Id,
                    ExpirationDate = DateTime.UtcNow.AddMinutes(15),
                };

                _appDbContext.ResetPasswordFields.Add(resets);
                await _appDbContext.SaveChangesAsync();

                SendRegistrationEmail(user.Email, user.FirstName, resets.PasswordGuid.ToString());
            }

            return Unit.Value;
        }

        private void SendRegistrationEmail(string userEmail, string firstName, string token)
        {
            var emailTemplate = _appDbContext.EmailTemplates.FirstOrDefault(x => x.Name == "ResetPasswordEmail");
            byte[] data = Convert.FromBase64String(emailTemplate.Body);
            string template = System.Text.Encoding.UTF8.GetString(data);

            template = template.Replace("{ResetPasswordLink}", HostName + $"/{token}")
                .Replace("{FirstName}", firstName);

            _emailService.QuickSendAsync(
                subject: "Reset Password Request!",
                body: template,
                to: userEmail);
        }
    }
    public class RequestResetPasswordCommandValidator : AbstractValidator<RequestResetPasswordCommand>
    {
        public RequestResetPasswordCommandValidator(IServiceProvider services)
        {

            RuleFor(d => d.Email)
                .NotEmpty()
                .EmailAddress();


            RuleFor(d => d).Custom((obj, context) =>
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                if (db == null)
                {
                    context.AddFailure("Internal problem - needs admin attention.");
                    return;
                }

                //var emailExists = db.Users.Any(d => d.Email == obj.Email);

                //if (!emailExists)
                //{
                //    context.AddFailure("There's a problem. Try again.");
                //}

            });
        }
    }
}
