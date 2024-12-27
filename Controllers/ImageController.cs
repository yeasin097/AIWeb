using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

public class ImageController : Controller
{
    private readonly HttpClient _httpClient;
    private const string ColabServerUrl = "https://ed3c-34-169-3-97.ngrok-free.app/generate"; // Replace with your ngrok URL

    public ImageController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    [HttpGet]
    public IActionResult GenerateImage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GenerateImage(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            ViewBag.ErrorMessage = "Prompt cannot be empty.";
            return View();
        }

        try
        {
            // Sending request to the ngrok-exposed Colab server
            var response = await _httpClient.PostAsJsonAsync(ColabServerUrl, new { prompt });

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Failed to generate image.";
                return View();
            }

            // Read the image as Base64 string
            var imageBase64 = await response.Content.ReadAsStringAsync();
            ViewBag.ImageBase64 = $"data:image/png;base64,{imageBase64}";
            ViewBag.Prompt = prompt; // Pass the prompt to the view
        }
        catch
        {
            ViewBag.ErrorMessage = "An error occurred while generating the image.";
        }

        return View();
    }

}
