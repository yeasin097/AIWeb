using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

public class LogoController : Controller
{
    //https://colab.research.google.com/drive/18au-Z53w8Q_qosxituOq65CZBfo5t2gH?usp=sharing GDrive
    //https://colab.research.google.com/drive/19c6nnEv7nZujb-sdL2RT053QWgyuOynj?usp=sharing download
    private readonly HttpClient _httpClient;
    private const string ColabServerUrl = "https://7987-34-125-65-87.ngrok-free.app/generate"; // Replace with your ngrok URL

    public LogoController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    [HttpGet]
    public IActionResult MakeLogo()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> MakeLogo(string companyName, string companyType)
    {
         if (string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(companyType))
        {
            ViewBag.ErrorMessage = "Please enter both company name and company type.";
            return View();
        }


        try
        {
            // Sending request to the ngrok-exposed Colab server
            string logoDescription = GenerateLogoDescription(companyName, companyType);
            var response = await _httpClient.PostAsJsonAsync(ColabServerUrl, new { logoDescription });

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Failed to generate image.";
                return View();
            }

            // Read the image as Base64 string
            var imageBase64 = await response.Content.ReadAsStringAsync();
            ViewBag.ImageBase64 = $"data:image/png;base64,{imageBase64}";
            ViewBag.LogoDescription = logoDescription; // Pass the prompt to the view
            ViewBag.CompanyName = companyName;
        }
        catch
        {
            ViewBag.ErrorMessage = "An error occurred while generating the image.";
        }

        return View();
    }

    private string GenerateLogoDescription(string companyName, string companyType)
    {
        // Formatting the description string
        return $"A logo for \"{companyName}\" with \"{companyName}\" written on it, which is a \"{companyType}\" based company.";
    }

}
