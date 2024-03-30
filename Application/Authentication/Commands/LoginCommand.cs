using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ClinicaWeb.Application.Authentication.Commands
{
    public class LoginCommand : IRequest<LoginResult>
    {
        public LoginCommand(UserLoginDto model)
        {
            Email = model.Email;
            Password = model.Password;
        }

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }
        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            LoginResult response = new LoginResult();

            var user = await _appDbContext.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                response.ResponseMessage = "Wrong credentials!";
                response.IsSuccessfull = false;
            }
            else if (!user.IsEmailVerified)
            {
                response.ResponseMessage = "Your email is not verified! Please check email for a verification code!";
                response.IsSuccessfull = false;
            }
            else if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.LoginAttempts++;
                response.ResponseMessage = "Wrong credentials!";
                response.IsSuccessfull = false;

                if (user.LoginAttempts >= 10)
                {
                    user.IsBlocked = true;
                    response.ResponseMessage = "You exceeded maximum number of login attempts! Your accound is blocked! Reset your password to restore account";
                }
                _appDbContext.SaveChanges();
            }
            else if (user.IsBlocked && VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.ResponseMessage = "Your account is blocked. Reset your password to restore access.";
                response.IsSuccessfull = false;
            }
            //else if(user.LoginsCount == 0 && user.Role == RoleEnum.SuperAdmin)
            //{
            //    response.IsFirstLogin = true;
            //}    
            else
            {
                response.Token = CreateToken(user);
                response.ResponseMessage = "Ura!";
                user.LoginsCount++;
                user.LoginAttempts = 0;
                _appDbContext.SaveChanges();
            }

            return response;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var result = computedHash.SequenceEqual(passwordHash);
                return result;
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("role", user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
