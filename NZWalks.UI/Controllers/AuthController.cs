using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;

namespace NZWalks.UI.Controllers
{
    public class AuthController : Controller
    {
        public const string TokenCookie = "NZWalksToken";
        public const string UserCookie = "NZWalksUser";

        private readonly IHttpClientFactory httpClientFactory;
        public AuthController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMessage = await client.PostAsJsonAsync("http://localhost:5144/api/Auth/Login", model);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var response = await httpResponseMessage.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (!string.IsNullOrEmpty(response?.JwtToken))
                    {
                        // API tokens expire after 15 minutes
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = Request.IsHttps,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                        };
                        Response.Cookies.Append(TokenCookie, response.JwtToken, cookieOptions);
                        Response.Cookies.Append(UserCookie, model.Username, cookieOptions);
                        return RedirectToAction("Index", "Regions");
                    }
                }

                ModelState.AddModelError(string.Empty, "Username or password is incorrect.");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Could not reach the NZ Walks API. Is it running?");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var httpResponseMessage = await client.PostAsJsonAsync("http://localhost:5144/api/Auth/Register", model);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful. Please log in.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, "Registration failed. The email may already be registered.");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Could not reach the NZ Walks API. Is it running?");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(TokenCookie);
            Response.Cookies.Delete(UserCookie);
            return RedirectToAction("Login");
        }
    }
}
