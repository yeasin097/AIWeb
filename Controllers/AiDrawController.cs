using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AiDraw.Controllers
{
    public class AiDrawController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AiDrawController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        // This action serves the Index view (HTML page)
        public IActionResult AiDraw()
        {
            return View();
        }

        // API endpoint to handle AI image generation
        [HttpPost("api/AiDraw/GenerateImage")]
        public async Task<IActionResult> GenerateImage([FromBody] AiRequest request)
        {
            string openAiUrl = "https://api.openai.com/v1/images/generations";

            var payload = new
            {
                prompt = request.Prompt,
                n = 1,
                size = "512x512"
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, openAiUrl);
            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
            httpRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return BadRequest(new { error = "Failed to generate image.", details = errorResponse });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return Ok(JsonConvert.DeserializeObject(responseBody));
        }
    }

    public class AiRequest
    {
        public string Prompt { get; set; }
    }
}
