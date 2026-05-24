using Application_Contract.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public JwtService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role?.Name ?? "User"),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)


            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
               expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            // حفظ التوكن في الداتا بيز
            _context.UserTokens.Add(new UserToken
            {
                Token = token,
                UserId = user.Id
            });
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> RevokeToken(string token)
        {
            var tokenRecord = await _context.UserTokens.FirstOrDefaultAsync(t => t.Token == token);

            if (tokenRecord != null)
            {
                _context.UserTokens.Remove(tokenRecord);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}