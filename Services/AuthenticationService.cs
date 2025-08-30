using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces.Repositories;
using LearnWebApi.Interfaces.Services;
using LearnWebApi.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace LearnWebApi.Services;

public class AuthenticationService : Interfaces.Services.IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly LinkGenerator _linker;

    public AuthenticationService(ITokenService tokenService, ICustomerRepository customerRepository,
        IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration, LinkGenerator linker)
    {
        _tokenService = tokenService;
        _customerRepository = customerRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
        _linker = linker;
    }

    public Task<Result<Customer>> GetCurrentLoginCustomerAsync(string token)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<CustomerLoginResponse>> LoginAsync(CustomerLoginRequest request)
    {
        try
        {
            Customer customer = await _customerRepository.GetCustomerByEmailAsync(request.Email);
            string hasPassword = await _customerRepository.GetHashPasswordByEmailAsync(request.Email);
            Console.WriteLine(hasPassword + " is has password");
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, hasPassword);
            if (!isPasswordValid)
                throw new Exception("Password is not valid");

            string accessToken = _tokenService.CreateToken(customer);
            RefreshToken refreshToken = _tokenService.CreateRefreshToken(customer.Id);
            await _refreshTokenRepository.AddAsync(refreshToken);
            return Result.Success(new CustomerLoginResponse(accessToken, refreshToken.Token));
        }
        catch (Exception ex)
        {
            return Result.Failure<CustomerLoginResponse>(new Error("404", ex.Message));
        }

    }

    public Result LoginGoogle()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = _linker.GetPathByName("GoogleCallback")
        };
        return Result.Success();
    }
    public Task<Result<string>> GoogleCallback()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TokenResponseModel>> RefreshToken(TokenRequestModel request)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var customerIdClaim = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (customerIdClaim is null)
                throw new Exception("Invalid access token");

            int customerId = int.Parse(customerIdClaim);
            RefreshToken? savedRefreshToken = await _refreshTokenRepository
                .GetRefreshTokenByCustomerIdAsync(customerId, request.RefreshToken);
            if (savedRefreshToken is null)
                throw new Exception("Refresh token is expired");

            if (savedRefreshToken.Expires <= DateTime.UtcNow)
                throw new Exception("Refresh token is expired");

            savedRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(savedRefreshToken);
            Customer customer = await _customerRepository.GetByIdAsync(customerId);
            var newAccessToken = _tokenService.CreateToken(customer);
            var newRefreshToken = _tokenService.CreateRefreshToken(customerId);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            TokenResponseModel response = new(newAccessToken, newRefreshToken.Token);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex.StackTrace!);
            return Result.Failure<TokenResponseModel>(new Error("400", ex.Message));
        }
    }

    public async Task<Result<Customer>> RegisterAsync(CustomerRegisterRequest request)
    {
        try
        {
            bool isEmailExist = await _customerRepository.IsCustomerEmailExistAsync(request.Email);
            if (isEmailExist)
                throw new Exception("Email already exist!");

            Customer newCustomer = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = request.Role ?? "",
                DateOfBirth = request.DateOfBirth,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12)
            };

            await _customerRepository.AddAsync(newCustomer);
            return Result.Success(newCustomer);
        }
        catch (Exception ex)
        {
            return Result.Failure<Customer>(new Error("400", ex.Message));
        }
    }

    // Hàm helper để đọc thông tin từ Access Token đã hết hạn
    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false // << Quan trọng: không kiểm tra thời gian hết hạn
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid access token");
        }

        return principal;
    }
}
