using LearnWebApi.Data;
using LearnWebApi.Endpoints;
using LearnWebApi.Interfaces;
using LearnWebApi.Services;
using LearnWebApi.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FluentValidation;
using LearnWebApi.Validators;
using LearnWebApi.Constraints;
using LearnWebApi.Interfaces.Repositories;
using LearnWebApi.Data.Repositories;
using Serilog;
using System.Diagnostics;
using Microsoft.AspNetCore.Antiforgery;
using LearnWebApi.Extensions;
using LearnWebApi.Middlewares;
using LearnWebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LearnWebApi.Interfaces.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using LearnWebApi.Athorization;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Init Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()                 // cho phép đính kèm context (path, user, v.v.)
    .WriteTo.Console()                       // ghi ra console ở dạng cấu trúc
    .MinimumLevel.Information()              // mức mặc định
    .CreateLogger();

builder.Host.UseSerilog();                   // thay logger mặc định của ASP.NET Core bằng Serilog

//database service
string connectionString = builder.Configuration.GetConnectionString("Docker")!;
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.Configure<ConfigOptions>(builder.Configuration.GetSection("MyOptions"));
builder.Services.AddSingleton<IRouteConstraint, EmailConstraint>();
builder.Services.AddDbContext<ProjectContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Route constraint service
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("email", typeof(EmailConstraint));
});

// Ad authentication service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options => // Thêm dịch vụ Cookie để quản lý session trong luồng redirect
{
    //  Luồng OAuth 2.0 hoạt động bằng cách chuyển hướng (redirect). 
    // Khi Google chuyển hướng người dùng trở lại ứng dụng của bạn, thông tin danh tính của họ 
    // (claims) cần được lưu tạm ở đâu đó. ASP.NET Core sử dụng một cookie tạm thời cho việc này. Đó là lý do chúng ta cần
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
})
.AddGoogle(options => // Thêm trình xác thực Google
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    
    // Khi Google gọi lại, nó sẽ gửi thông tin vào một cookie tạm thời.
    // Chúng ta chỉ định scheme cookie đã đăng ký ở trên để xử lý việc này.
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
});
// Authorization
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, EmailDomainHandler>();
builder.Services.AddAuthorization(Options =>
{
    Options.AddPolicy("AtLeast18", policy =>
    {
        policy.Requirements.Add(new MinimumAgeRequirement(18));
    });
    Options.AddPolicy("AdminRole", policy =>
    {
        policy.RequireRole("Admin");
    });
    Options.AddPolicy("PremiumCustomer", policy =>
    {
        policy.RequireClaim("SubscriptionLevel", "Premium");
    });
    Options.AddPolicy("GoogleDomain", policy =>
    {
        policy.Requirements.Add(new EmailDomainRequirement("@gmail.com"));
    });
});
//validation fluent 
builder.Services.AddValidatorsFromAssemblyContaining<CustomerRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequestValidator>();

// using caching 
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

//Global exception
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Bước 1: Định nghĩa Security Scheme
    // Ta định nghĩa một "cơ chế bảo mật" mới tên là "Bearer".
    // Cơ chế này dùng API Key (chính là token) và nó được gửi trong Header theo chuẩn "Bearer".
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Bước 2: Áp dụng Security Scheme cho tất cả các endpoint
    // Ta yêu cầu Swagger phải sử dụng cơ chế "Bearer" đã định nghĩa ở trên.
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

//Antiforgery 
builder.Services.AddAntiforgery();

// Middleware
builder.Services.AddSingleton<RequestCountingMiddlware>();
builder.Services.AddApiKeyAuthentication();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Content-Type", "Authorization");
        }
    );
    options.AddPolicy("AllowAnyOrigin",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Filter
builder.Services.AddScoped<LogEndpointExecutionFilter>();

// test environment variable
var myEnvValue = builder.Configuration["FIRST_ENVIRONMENT"];

// AFTER BUILD:
var app = builder.Build();

// exeption handler
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

// Middleware to display request timeline
// app.UseSerilogRequestLogging(); or
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    await next(context);
    sw.Stop();

    Log.ForContext("path", context.Request.Path)
        .ForContext("status", context.Response.StatusCode)
        .ForContext("elapsedMs", sw.ElapsedMilliseconds)
        .Information("HTTP {Method} {Path} -> {Status} in {Elapsed} ms",
        context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
});
// app.UseHeaderModify();
// app.UseRequestCounting();

//ENDPOINTS
app.MapProductEndpoints("/products");
app.MapCustomerEndpoints("/customers");
app.MapAuthenticationEndpoints("/auth");

// antiforgery token
app.MapGet("/antiforgery/token", (IAntiforgery antiforgery, HttpContext httpContext) =>
{
    var tokens = antiforgery.GetAndStoreTokens(httpContext);
    return Results.Ok(new
    {
        RequestToken = tokens.RequestToken
    });
});

// LEARNING ENDPOINTS
app.MapGet("google-email-domain", () => "Your email domain is @gmail.com").RequireAuthorization("GoogleDomain");
app.MapGet("/premium-customer-only", () => "You are premium customer").RequireAuthorization("PremiumCustomer");
app.MapGet("/multi-authorization-requirement-test", () => "There are two authorization requirement in this api")
    .RequireAuthorization("AdminRole", "AtLeast18");
app.MapGet("/minimum-age-authorization-test", () => "You are over 18").RequireAuthorization("AtLeast18");

app.MapGet("/emailTest/{email:email}", (string email) =>
{
    return $"This is surely an email: {email}";
});

app.MapGet("/auth/test", () => "This is authentication api").RequireAuthorization();
app.MapGet("/rangeTest/{num:int:range(18,65)}", (int num) =>
{
    return $"Your age is {num}";
});
app.MapGet("/test/search/{name?}", (string? name) =>
{
    if (name is null)
        return Results.Ok("you will get all products");

    return Results.Ok($"you search for product name {name}, right?");
});
app.MapGet("/sanpham/{pageNumber:int=1}", (int pageNumber) =>
{
    return $"Page number: {pageNumber}";
});
app.MapGet("/files/{*path}", (string path) =>
{
    return $"File path: {path.Split('/')[0]}";
});
app.MapGet("/", () => "Hello World!").AddEndpointFilter<LogEndpointExecutionFilter>();
app.MapGet("/config", (IConfiguration config) =>
{
    return Results.Ok($"My config text: {config["MyText"]}");
});
app.MapGet("/env", () => $"CHECK ENVRIONNMENT VARIABLE: {myEnvValue}");
app.MapGet("/my-config-options", (IOptions<ConfigOptions> options) =>
{
    if (options is not null)
    {
        return Results.Ok($"My options: {options.Value.OwnerName}, {options.Value.Age}");
    }
    return Results.BadRequest();
});

app.Run();