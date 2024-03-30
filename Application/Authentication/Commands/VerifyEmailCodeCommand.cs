using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Users;
using MediatR;

namespace ClinicaWeb.Application.Authentication.Commands
{
    public class VerifyEmailCodeCommand : IRequest<bool>
    {
        public VerifyEmailCodeCommand(VerifyEmailByCodeDto model)
        {
            Email = model.Email;
            VerificationCode = model.VerificationCode;
        }
        public string Email { get; set; }
        public Guid? VerificationCode { get; set; }
    }

    public class VerifyEmailCodeCommandHandler : IRequestHandler<VerifyEmailCodeCommand, bool>
    {
        private readonly AppDbContext _appDbContext;

        public VerifyEmailCodeCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> Handle(VerifyEmailCodeCommand request, CancellationToken cancellationToken)
        {
            var user = _appDbContext.Users.Where(x => x.Email == request.Email && x.VerificationCode == request.VerificationCode).FirstOrDefault();
            var isVerified = false;

            if (user != null)
            {
                user.IsEmailVerified = true;
                isVerified = true;
            }
            await _appDbContext.SaveChangesAsync();

            return isVerified;
        }
    }
}
