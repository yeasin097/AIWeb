using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TextToAudioWebApp.Services
{
    public class OpenAIAPI
    {
        private readonly string _apiKey;
        private readonly string _apiEndpoint;

        public OpenAIAPI(string apiKey, string apiEndpoint)
        {
            _apiKey = apiKey;
            _apiEndpoint = apiEndpoint;
        }

        public async Task<string> GetResponseAsync(string model, string prompt)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var request = new
            {
                model,
                prompt,
                max_tokens = 100
            };

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }
    }



    public class HuggingFaceAPI
    {
        private readonly string _apiKey = "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"; // Replace with your Hugging Face API key
        private readonly string _apiEndpoint = "https://api-inference.huggingface.co/models/facebook/blenderbot-400M-distill"; // Model endpoint

        public async Task<string> GetResponseAsync(string inputText)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var request = new
            {
                inputs = inputText
            };

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }
    }
}