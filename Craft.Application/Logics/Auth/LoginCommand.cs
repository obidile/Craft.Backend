using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Microsoft.Extensions.Configuration;
using CryptoHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Craft.Application.Logics.Auth
{
    public class LoginCommand : IRequest<string>
    {
        public string MailAddress { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IConfiguration _config;
        private readonly IApplicationContext _dbContext;

        public LoginCommandHandler(IConfiguration config, IApplicationContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.MailAddress.ToLower() == request.MailAddress.ToLower(), cancellationToken);

            if (user == null || !Crypto.VerifyHashedPassword(user.PasswordHush, request.Password))
            {
                return "Wrong MailAddress or Password. Please try again.";
            }

            if (user.Deactivated)
            {
                return "Your account has been suspended. Send a reactivation mail to craftreactivation@gmail.com";
            }

            var jwtToken = GenerateToken(user);

            if (jwtToken == null)
            {
                return "Invalid login attempt.";
            }

            return jwtToken;
        }

        private string GenerateToken(User user) // This Implmentation allows Token expires in 30 days
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JwtConfig:Secret") ?? "default-secret-key");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.MailAddress),
                new Claim("userId", user.Id.ToString()),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim("email", user.MailAddress),
            };

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                claims.Add(new Claim("phoneNumber", user.PhoneNumber));
            }

            var nowUtc = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = nowUtc,
                Expires = nowUtc.AddDays(30),
                Issuer = _config.GetValue<string>("JwtConfig:Issuer"),
                Audience = _config.GetValue<string>("JwtConfig:Audience"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
