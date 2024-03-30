using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Users;
using ClinicaWeb.Shared.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace ClinicaWeb.Application.Users.Commands
{
    public class EditPasswordCommand : IRequest<string>
    {
        public EditPasswordCommand(ChangePasswordDto model)
        {
            Id = model.Id;
            Email = model.Email;
            CurrentPassword = model.CurrentPassword;
            NewPassword = model.NewPassword;
            ConfirmPassword = model.ConfirmPassword;
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class EditPasswordCommandHandler : IRequestHandler<EditPasswordCommand, string>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditPasswordCommandHandler(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(EditPasswordCommand request, CancellationToken cancellationToken)
        {
            var claimUserid = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            int.TryParse(claimUserid, out int claimUseridAsInt);
            var user = _appDbContext.Users.Where(u => u.Id == claimUseridAsInt).FirstOrDefault();

            var userToEdit = _appDbContext.Users.FirstOrDefault(c => c.Id == request.Id || c.Email == request.Email);

            if (userToEdit.Id == claimUseridAsInt)
            {
                if (!VerifyPasswordHash(request.CurrentPassword, userToEdit.PasswordHash, userToEdit.PasswordSalt))
                {
                    throw new InvalidOperationException("Old password does not match.");
                }

                CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                userToEdit.PasswordHash = passwordHash;
                userToEdit.PasswordSalt = passwordSalt;
                userToEdit.LoginsCount++;
            }
            else if (user != null && user.Role == Role.Administrator)
            {
                CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                userToEdit.PasswordHash = passwordHash;
                userToEdit.PasswordSalt = passwordSalt;
                userToEdit.LoginsCount++;
            }

            _appDbContext.SaveChanges();

            return "Password was reseted";
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
