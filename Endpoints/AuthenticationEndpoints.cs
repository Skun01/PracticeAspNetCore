using System;
using System.Security.Claims;
using LearnWebApi.DTOs.Customer;
using LearnWebApi.Entities;
using LearnWebApi.Interfaces.Repositories;
using LearnWebApi.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace LearnWebApi.Endpoints;

public static class AuthenticationEndpoints
{
    public static RouteGroupBuilder MapAuthenticationEndpoints(this WebApplication app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Authentication");
        group.MapPost("/register", async ([FromBody] CustomerRegisterRequest request, Interfaces.Services.IAuthenticationService authService) =>
        {
            var result = await authService.RegisterAsync(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        group.MapPost("/login", async ([FromBody] CustomerLoginRequest request, Interfaces.Services.IAuthenticationService authService) =>
        {
            var result = await authService.LoginAsync(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        group.MapPost("/token/refresh", async ([FromBody] TokenRequestModel request, Interfaces.Services.IAuthenticationService authService) =>
        {
            var result = await authService.RefreshToken(request);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });

        group.MapGet("/login-google", () =>
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/auth/signin-google-callback"
            };
            return Results.Challenge(properties, [GoogleDefaults.AuthenticationScheme]);
        });

        group.MapGet("/signin-google-callback", async (HttpContext context,
            ICustomerRepository customerRepository, ITokenService tokenService) =>
        {
            // 1. Lấy kết quả xác thực từ cookie tạm thời
            //    ASP.NET Core middleware đã tự động xử lý việc trao đổi code lấy access token từ Google
            //    và dùng nó để lấy thông tin user, sau đó tạo principal và lưu vào cookie.
            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
                return Results.BadRequest("Lỗi khi xác thực với Google.");

            // 2. Lấy thông tin (claims) của người dùng từ Google
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var givenName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var surname = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Results.BadRequest("Không lấy được thông tin email từ Google.");
            }

            // 3. Logic nghiệp vụ: Kiểm tra user trong DB, nếu chưa có thì tạo mới
            //    Đây là phần "cây cầu" quan trọng nhất
            var customer = await customerRepository.GetCustomerByEmailAsync(email);
            if (customer == null)
            {
                // Nếu người dùng chưa tồn tại, tạo một tài khoản mới cho họ
                customer = new Customer
                {
                    Email = email,
                    FirstName = givenName,
                    LastName = surname
                };
                await customerRepository.AddAsync(customer);
            }

            // 4. Tạo JWT của ứng dụng bạn cho người dùng này
            var appJwtToken = tokenService.CreateToken(customer);

            // 5. Trả về token cho client
            // Cách trả về token tùy thuộc vào kiến trúc của bạn.
            // Phổ biến là redirect về một trang trên Frontend và gửi token qua query string.
            // Frontend sẽ đọc token từ URL, lưu vào localStorage và chuyển người dùng đến trang chính.
            var frontendCallbackUrl = $"https://your-frontend-app.com/auth/callback?token={appJwtToken}";
            return Results.Redirect(frontendCallbackUrl);
        });
        return group;
    }
}
