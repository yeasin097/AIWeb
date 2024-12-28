using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MyMvcApp.Controllers
{
    public class AudioController : Controller
    {

        //https://colab.research.google.com/drive/1cA9hloozYVDBPn8ekWmvLFNPfHgnqiU-?usp=sharing
        private readonly HttpClient _httpClient;

        public AudioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAudio(GenerateAudioRequest request)
        {
            try
            {
                // Serialize the request object to JSON
                var json = JsonConvert.SerializeObject(request);
                System.Diagnostics.Debug.WriteLine(json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request to Python API
                var response = await _httpClient.PostAsync("https://da7d-34-45-124-139.ngrok-free.app/run_tts", content);

                if (response.IsSuccessStatusCode)
                {
                    // Read the audio file bytes
                    var audioBytes = await response.Content.ReadAsByteArrayAsync();

                    // Convert bytes to base64
                    var audioBase64 = Convert.ToBase64String(audioBytes);

                    // Pass the base64 string to the view
                    ViewBag.AudioBase64 = $"data:audio/wav;base64,{audioBase64}";
                    return View();
                }

                // Handle errors
                var errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Error from API: {errorMessage}";
                return View();
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult GenerateAudio()
        {
            return View();
        }



    }
}
