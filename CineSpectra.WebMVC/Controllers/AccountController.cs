using CineSpectra.Application.DTOs;
using CineSpectra.WebMVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineSpectra.WebMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var authResponse = await _apiService.LoginAsync(model);
            if (!authResponse.IsSuccess || string.IsNullOrEmpty(authResponse.Token))
            {
                ModelState.AddModelError(string.Empty, authResponse.Message ?? "Giriş başarısız.");
                return View(model);
            }

            // Cookie Claims oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authResponse.UserId!),
                new Claim(ClaimTypes.Name, authResponse.UserName!),
                new Claim("JwtToken", authResponse.Token)
            };

            foreach (var role in authResponse.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (authResponse.Roles.Contains("Admin") && string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Admin");

            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.RegisterAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Kayıt başarısız.");
                return View(model);
            }

            return RedirectToAction("Login", new { message = "Kayıt başarılı! Giriş yapabilirsiniz." });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}