using FilmLibraryManagement;
using FilmLibraryManagement.Data;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Add Services to the Container
// ========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ========================================
// Configure Database Connection
// ========================================
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================
// Register Repositories
// ========================================
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IDirectorRepository, DirectorRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<AuthController>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// ========================================
// Configure Authentication
// ========================================
builder.Services.AddAuthentication(options =>
{
    // Use cookies for the initial authentication flow (Auth0 redirect)
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // This will be invoked on 401 responses
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
.AddCookie()
.AddJwtBearer(options =>
{
    options.Authority = "https://dev-6bv2f6383lp7dbrb.us.auth0.com/";
    options.Audience = "https://film-library.com/api";
})
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.CallbackPath = "/callback"; // Must match Auth0 dashboard
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.Name
    };
});


builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

// ========================================
// Configure Authorization
// ========================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("delete:directors", policy =>
        policy.RequireClaim("permissions", "delete:directors"));
});

// Apply Global Authentication Policy (All Controllers Require Authentication)
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// ========================================
// Enable CORS
// ========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // Change this to your frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Allow credentials like cookies or headers if necessary
    });
});

var app = builder.Build();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// ========================================
// Configure Middleware
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication(); // This must come before UseAuthorization()
app.UseAuthorization();
app.MapControllers();
app.Run();
