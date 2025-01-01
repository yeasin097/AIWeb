using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TextToAudioWebApp.Models;
using TextToAudioWebApp.Services;

namespace TextToAudioWebApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly OpenAIAPI _openAIAPI;
        private static List<ChatMessage> ChatHistory = new();

        public ChatController(OpenAIAPI openAIAPI)
        {
            _openAIAPI = openAIAPI;
        }

        [HttpGet]
        public IActionResult Chat()
        {
            ViewBag.ChatHistory = ChatHistory;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Chat(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return RedirectToAction("Chat");
            }

            // Add user input to history
            ChatHistory.Add(new ChatMessage { Role = "User", Content = prompt });

            try
            {
                var response = await _openAIAPI.GetResponseAsync("text-davinci-003", prompt);
                ChatHistory.Add(new ChatMessage { Role = "AI", Content = response });
            }
            catch (System.Exception ex)
            {
                ChatHistory.Add(new ChatMessage { Role = "AI", Content = $"Error: {ex.Message}" });
            }

            ViewBag.ChatHistory = ChatHistory;
            return View();
        }
    }
}
