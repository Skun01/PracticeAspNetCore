namespace LearnWebApi.DTOs.Customer;

public record class TokenResponseModel(
    string AccessToken, 
    string RefreshToken
);
