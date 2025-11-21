using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }


        public async Task<IActionResult> Index()
        {
            var regions = new List<RegionDto>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpresponse = await client.GetAsync("http://localhost:5144/api/Regions");
                httpresponse.EnsureSuccessStatusCode();
                regions.AddRange(await httpresponse.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
            }
            catch(Exception ex)
            {
                // Handle exception (e.g., log it)
            }

            return View(regions);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel model)
        {
            var client = httpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:5144/api/Regions"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage =  await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();
            if (response != null)
            {
                return RedirectToAction("Index","Regions");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<RegionDto>($"http://localhost:5144/api/Regions/{id.ToString()}");

            if (response is not null)
            {
                return View(response);
            }

            return View(null);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto request)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"http://localhost:5144/api/Regions/{request.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var respose = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (respose is not null)
            {
                return RedirectToAction("Index", "Regions");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync($"http://localhost:5144/api/Regions/{request.Id}");

                httpResponseMessage.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Regions");
            }
            catch (Exception ex)
            {
                // Console
            }

            return View("Edit");
        }
    }
}
