using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Net;
using System.Net.Http.Headers;

namespace NZWalks.UI.Controllers
{
    public class DifficultiesController : Controller
    {
        // The API has no Difficulties endpoint, so difficulties are read from the walks that use them
        private const string WalksUrl = "http://localhost:5144/api/Walks";

        private readonly IHttpClientFactory httpClientFactory;
        public DifficultiesController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // Sends the JWT saved at login so the API's [Authorize] checks pass
        private HttpClient CreateApiClient()
        {
            var client = httpClientFactory.CreateClient();
            var token = Request.Cookies[AuthController.TokenCookie];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.ContainsKey(AuthController.TokenCookie))
            {
                return RedirectToAction("Login", "Auth");
            }

            var difficulties = new List<DifficultyViewModel>();
            try
            {
                var client = CreateApiClient();
                var httpresponse = await client.GetAsync(WalksUrl);
                if (httpresponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Token missing or expired (API tokens last 15 minutes)
                    return RedirectToAction("Login", "Auth");
                }
                if (httpresponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    TempData["ErrorMessage"] = "Your account needs the Reader or Writer role to view difficulties.";
                    return View(difficulties);
                }
                httpresponse.EnsureSuccessStatusCode();

                var walks = await httpresponse.Content.ReadFromJsonAsync<IEnumerable<WalkDto>>() ?? Enumerable.Empty<WalkDto>();
                difficulties.AddRange(walks
                    .Where(w => w.Difficulty != null)
                    .GroupBy(w => w.DifficultyId)
                    .Select(g => new DifficultyViewModel
                    {
                        Id = g.Key,
                        Name = g.First().Difficulty.Name,
                        Walks = g.OrderBy(w => w.Name).ToList()
                    })
                    .OrderBy(d => d.Name));
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Could not load difficulties. Is the NZ Walks API running?";
            }

            return View(difficulties);
        }
    }
}
