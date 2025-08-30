namespace LearnWebApi.DTOs.Customer;

public record class CustomerLoginResponse(
    string AccessToken,
    string RefreshToken
);
