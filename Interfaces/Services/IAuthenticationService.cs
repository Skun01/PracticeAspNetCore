using System;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;
using LearnWebApi.Shared;

namespace LearnWebApi.Interfaces.Services;

public interface IAuthenticationService
{
    Task<Result<CustomerLoginResponse>> LoginAsync(CustomerLoginRequest request);
    Task<Result<Customer>> RegisterAsync(CustomerRegisterRequest request);
    Task<Result<Customer>> GetCurrentLoginCustomerAsync(string token);
    Task<Result<TokenResponseModel>> RefreshToken(TokenRequestModel request);
    Result LoginGoogle();
    Task<Result<string>> GoogleCallback();

}
