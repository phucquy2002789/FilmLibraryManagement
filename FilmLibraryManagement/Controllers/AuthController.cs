using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration; // Inject configuration
    }

    // 🔹 Login for web app users
    [HttpGet("login")]
    public IActionResult Login()
    {
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback), "Auth") // Crucial redirect
        };

        return Challenge(authenticationProperties, "Auth0"); // Use "Auth0" instead of OpenIdConnectDefaults.AuthenticationScheme
    }


    // 🔹 Process Auth0 login callback
    [HttpGet("callback")]
    public async Task<IActionResult> Callback()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return BadRequest(new { error = "Authentication failed." });
        }

        // ✅ Return user claims instead of redirecting
        var claims = result.Principal?.Identities.FirstOrDefault()?.Claims
            .Select(c => new { c.Type, c.Value });

        return Ok(new
        {
            message = "Login successful",
            access_token = result.Properties.GetTokenValue("access_token"),
            id_token = result.Properties.GetTokenValue("id_token"),
            claims
        });
    }

    // 🔹 Secure API - Get User Profile (JWT Required)
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(new { message = "Authenticated API request", userClaims });
    }

    // 🔹 Logout for both web & API users
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        var auth0Domain = _configuration["Auth0:Domain"];
        var clientId = _configuration["Auth0:ClientId"];
        var returnTo = "https://localhost:7038/swagger/index.html"; // Redirect after logout

        var logoutUrl = $"https://{auth0Domain}/v2/logout?" +
                        $"client_id={clientId}" +
                        $"&returnTo={Uri.EscapeDataString(returnTo)}";

        return SignOut(new AuthenticationProperties { RedirectUri = logoutUrl }, CookieAuthenticationDefaults.AuthenticationScheme);
    }

}
