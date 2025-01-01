using Microsoft.AspNetCore.Mvc;
using TextToAudioWebApp.Services;

namespace TextToAudioWebApp.Controllers
{
    public class ChatHuggingController : Controller
    {
        private readonly HuggingFaceAPI _huggingFaceAPI;

        public ChatHuggingController(HuggingFaceAPI huggingFaceAPI)
        {
            _huggingFaceAPI = huggingFaceAPI;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(string inputText)
        {
            var response = await _huggingFaceAPI.GetResponseAsync(inputText);
            return Content(response);
        }

        public IActionResult Chat()
        {
            return View();
        }
    }
}
