using System;

namespace LearnWebApi.DTOs.Customer;

public record TokenRequestModel(
    string AccessToken, 
    string RefreshToken
);
