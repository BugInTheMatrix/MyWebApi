using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NZWalks.UI.Controllers
{
    public class WalksController : Controller
    {
        private const string WalksUrl = "http://localhost:5144/api/Walks";
        private const string RegionsUrl = "http://localhost:5144/api/Regions";

        // The API has no Difficulties endpoint; these match the HasData seed in NZWalksDbContext
        private static readonly List<SelectListItem> Difficulties = new()
        {
            new SelectListItem("Easy", "54466f17-02af-48e7-8ed3-5a4a8bfacf6f"),
            new SelectListItem("Medium", "ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"),
            new SelectListItem("Hard", "f808ddcd-b5e5-4d80-b732-1ca523e48434")
        };

        private readonly IHttpClientFactory httpClientFactory;
        public WalksController(IHttpClientFactory httpClientFactory)
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

        // Fills the Region and Difficulty dropdowns used by the Add and Edit views
        private async Task LoadDropdownsAsync(HttpClient client)
        {
            var regions = await client.GetFromJsonAsync<IEnumerable<RegionDto>>(RegionsUrl) ?? Enumerable.Empty<RegionDto>();
            ViewBag.Regions = regions.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
            ViewBag.Difficulties = Difficulties;
        }

        private static StringContent ToJson(WalkFormViewModel model)
        {
            return new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        }

        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.ContainsKey(AuthController.TokenCookie))
            {
                return RedirectToAction("Login", "Auth");
            }

            var walks = new List<WalkDto>();
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
                    TempData["ErrorMessage"] = "Your account needs the Reader or Writer role to view walks.";
                    return View(walks);
                }
                httpresponse.EnsureSuccessStatusCode();
                walks.AddRange(await httpresponse.Content.ReadFromJsonAsync<IEnumerable<WalkDto>>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Could not load walks. Is the NZ Walks API running?";
            }

            return View(walks);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (!Request.Cookies.ContainsKey(AuthController.TokenCookie))
            {
                return RedirectToAction("Login", "Auth");
            }

            await LoadDropdownsAsync(CreateApiClient());
            return View(new WalkFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(WalkFormViewModel model)
        {
            var client = CreateApiClient();
            var httpResponseMessage = await client.PostAsync(WalksUrl, ToJson(model));
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                ModelState.AddModelError(string.Empty, "Your account needs the Writer role to add walks.");
                await LoadDropdownsAsync(client);
                return View(model);
            }
            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError(string.Empty, "The API rejected the walk. Check that every field is filled in.");
                await LoadDropdownsAsync(client);
                return View(model);
            }
            httpResponseMessage.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Walks");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = CreateApiClient();
            var httpResponseMessage = await client.GetAsync($"{WalksUrl}/{id}");
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "That walk no longer exists.";
                return RedirectToAction("Index", "Walks");
            }
            httpResponseMessage.EnsureSuccessStatusCode();

            var walk = await httpResponseMessage.Content.ReadFromJsonAsync<WalkDto>();
            await LoadDropdownsAsync(client);
            return View(new WalkFormViewModel
            {
                Id = walk.Id,
                Name = walk.Name,
                Description = walk.Description,
                LengthInKm = walk.LengthInKm,
                WalkImageUrl = walk.WalkImageUrl,
                DifficultyId = walk.DifficultyId,
                RegionId = walk.RegionId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WalkFormViewModel model)
        {
            var client = CreateApiClient();
            var httpResponseMessage = await client.PutAsync($"{WalksUrl}/{model.Id}", ToJson(model));
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                ModelState.AddModelError(string.Empty, "Your account needs the Writer role to edit walks.");
                await LoadDropdownsAsync(client);
                return View(model);
            }
            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "That walk no longer exists.";
                return RedirectToAction("Index", "Walks");
            }
            httpResponseMessage.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Walks");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var client = CreateApiClient();
                var httpResponseMessage = await client.DeleteAsync($"{WalksUrl}/{id}");

                if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    TempData["ErrorMessage"] = "Your account needs the Writer role to delete walks.";
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "That walk no longer exists.";
                }
                else
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Could not delete the walk. Is the NZ Walks API running?";
            }

            return RedirectToAction("Index", "Walks");
        }
    }
}
