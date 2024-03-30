using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Users;
using MediatR;
using System.Security.Cryptography;

namespace ClinicaWeb.Application.Authentication.Commands
{
    public class ResetPasswordCommand : IRequest<ResetPasswordResultDto>
    {
        public ResetPasswordCommand(ResetPasswordRequestDto request)
        {
            ResetGuid = request.ResetGuid;
            Password = request.Password;
        }
        public Guid ResetGuid { get; set; }
        public string Password { get; set; }


    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResultDto>
    {
        private readonly AppDbContext _appDbContext;
        public ResetPasswordCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<ResetPasswordResultDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var userToReset = _appDbContext.ResetPasswordFields.Where(g => g.PasswordGuid == request.ResetGuid).FirstOrDefault();

            if (DateTime.UtcNow > userToReset.ExpirationDate)
            {

                return new ResetPasswordResultDto { IsSuccess = false, Message = "Reset password link has expired. Try again!" };
            }
            if (userToReset != null)
            {
                var user = _appDbContext.Users.Where(u => u.Id == userToReset.UserId).FirstOrDefault();

                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.IsBlocked = false;
                user.LoginAttempts = 0;
            }
            else
            {
                return new ResetPasswordResultDto { IsSuccess = false, Message = "Something went wrong!" };
            }

            _appDbContext.SaveChanges();
            return new ResetPasswordResultDto { IsSuccess = true, Message = "Your password was succesfully reseted!" };

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

}
