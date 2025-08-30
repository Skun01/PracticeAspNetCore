using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace LearnWebApi.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _secretKey;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
    }

    public string CreateToken(Customer customer)
    {
        var credenticals = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.DateOfBirth, customer.DateOfBirth.ToString()!),
            new Claim(ClaimTypes.Role, customer.Role),
            new Claim("SubscriptionLevel", "Premium")
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpirationHours"]!)),
            signingCredentials: credenticals
            );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
    public RefreshToken CreateRefreshToken(int customerId)
    {
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            return new RefreshToken
            {
                CustomerId = customerId,
                Token = token,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
            };
        }
    }
}
