using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using CVManager.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CVManager.Controllers;

public class AuthController : ControllerBase
{
    
    public IActionResult Login()
    {
        var claimUser = HttpContext.User;
        if (claimUser.Identity is { IsAuthenticated: true })
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserCredentials userCredentials)
    {
        if (((userCredentials.Username == "admin") || ((userCredentials.Username == "user"))) && (userCredentials.Password == "123456"))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userCredentials.Username)
            };
            if (userCredentials.Username == "admin")
            {
                claims.Add(new Claim("Editor", ""));
            }

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = userCredentials.KeepLoggedIn
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("Index", "Home");
        }
        SetFlash("Invalid user name or password", "flash--danger");
        return RedirectToAction("Login", "Auth");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}