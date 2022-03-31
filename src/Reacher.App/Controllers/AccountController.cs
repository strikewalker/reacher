using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Reacher.App.Controllers;

public class AccountController : Controller
{
    [HttpGet()]
    public IActionResult Login()
    {
        if (HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
        }

        return Redirect("/setup");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        return Redirect("/");
    }
}
