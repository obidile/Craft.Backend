using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Microsoft.Extensions.Configuration;
using CryptoHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MXTires.Microdata.Core;
using System.Net.Http;

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
                return "Your account has been suspended";
            }

            var jwtToken = GenerateToken(user);

            if (jwtToken == null)
            {
                return "Invalid login attempt.";
            }

            return jwtToken;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("Jwt:SecretKey"));

            var expiryTime = _config.GetValue<int>("Jwt:ExpiryTime");

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

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryTime),
                Issuer = _config.GetValue<string>("JwtConfig:Issuer"),
                Audience = _config.GetValue<string>("JwtConfig:Audience"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
