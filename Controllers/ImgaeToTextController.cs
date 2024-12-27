using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

public class ImageToTextController : Controller
{
    private readonly HttpClient _httpClient;
    private const string ColabServerUrl = "https://5a5c-34-106-88-182.ngrok-free.app/process-image"; // Replace with your ngrok URL

    public ImageToTextController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet]
    public IActionResult ImageToText()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ImageToText(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            ViewBag.ErrorMessage = "Please upload a valid image file.";
            return View();
        }

        try
        {
            // Convert uploaded image to byte array
            var imageBytes = await ConvertImageToByteArrayAsync(image);

            // Send image to Colab server and get response
            var response = await SendImageToServerAsync(imageBytes, image.FileName);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Failed to process the image. Please try again later.";
                return View();
            }

            var resultText = await response.Content.ReadAsStringAsync();

            // Set the view data for image and text
            ViewBag.ImageBase64 = ConvertToBase64Image(imageBytes);
            ViewBag.ResultText = resultText;
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
        }

        return View();
    }

    // Helper Method: Convert image to byte array
    private async Task<byte[]> ConvertImageToByteArrayAsync(IFormFile image)
    {
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    // Helper Method: Send image to Colab server
    private async Task<HttpResponseMessage> SendImageToServerAsync(byte[] imageBytes, string fileName)
    {
        var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(imageBytes), "image", fileName }
        };
        return await _httpClient.PostAsync(ColabServerUrl, content);
    }

    // Helper Method: Convert byte array to Base64 string for image display
    private string ConvertToBase64Image(byte[] imageBytes)
    {
        return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
    }
}
