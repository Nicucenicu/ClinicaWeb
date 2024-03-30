using ClinicaWeb.Persistence;
using MediatR;

namespace ClinicaWeb.Application.Authentication.Commands
{
    public class VerifyEmailThroughLinkCommand : IRequest<bool>
    {
        public VerifyEmailThroughLinkCommand(string? token)
        {
            Token = token;
        }
        public string? Token { get; set; }
    }

    public class VerifyEmailThroughLinkCommandHandler : IRequestHandler<VerifyEmailThroughLinkCommand, bool>
    {
        private readonly AppDbContext _appDbContext;

        public VerifyEmailThroughLinkCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> Handle(VerifyEmailThroughLinkCommand request, CancellationToken cancellationToken)
        {
            var user = _appDbContext.Users.Where(x => x.VerificationToken == request.Token).FirstOrDefault();
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
