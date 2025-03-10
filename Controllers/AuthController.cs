using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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
            RedirectUri = Url.Action(nameof(Callback), "Auth") //  crucial redirect
        };

        // Directly challenge with the OpenID Connect authentication scheme ("Auth0")
        return Challenge(authenticationProperties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    // 🔹 Auth0 callback (process login result)
    [HttpGet("callback")]
    public async Task<IActionResult> Callback()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return BadRequest("Authentication failed.");
        }

        // Store the JWT token from Auth0 in the session or send it to the client
        var accessToken = result.Properties.GetTokenValue("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("No access token received.");
        }

        return Redirect("https://localhost:7038/swagger/index.html"); // Redirect after login
    }
    //[HttpGet("callback")]
    //public async Task<IActionResult> Callback()
    //{
    //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //    if (!result.Succeeded)
    //    {
    //        return BadRequest(new { error = "Authentication failed." });
    //    }

    //    // ✅ Return user claims instead of redirecting
    //    var claims = result.Principal?.Identities.FirstOrDefault()?.Claims
    //        .Select(c => new { c.Type, c.Value });

    //    return Ok(new { message = "Login successful", claims });
    //}

    // 🔹 Logout and redirect
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
