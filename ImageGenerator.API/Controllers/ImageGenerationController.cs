using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageGenerator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageGenerationController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ImageGenerationController> _logger;
        private readonly IConfiguration _configuration;

        public ImageGenerationController(
            IHttpClientFactory clientFactory,
            ILogger<ImageGenerationController> logger,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateImage([FromBody] ImagePrompt prompt)
        {
            try
            {
                var apiKey = _configuration["HuggingFace:ApiKey"] ??
                            _configuration["HuggingFace:apiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("API key not found");
                    return StatusCode(500, "API configuration error");
                }

                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Skapa request
                var requestContent = new
                {
                    inputs = prompt.Text,
                    wait_for_model = true
                };

                var response = await client.PostAsJsonAsync(
                    "https://api-inference.huggingface.co/models/black-forest-labs/FLUX.1-schnell",
                    requestContent
                );

                // Läs response som bytes först
                var responseBytes = await response.Content.ReadAsByteArrayAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorText = System.Text.Encoding.UTF8.GetString(responseBytes);
                    _logger.LogError($"API Error: {errorText}");
                    return StatusCode((int)response.StatusCode, errorText);
                }

                // Konvertera bytes till base64
                var base64String = Convert.ToBase64String(responseBytes);

                return Ok(new ImageGenerationResult
                {
                    b64_json = base64String
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }

    public class ImagePrompt
    {
        public string Text { get; set; }
    }

    public class ImageGenerationResult
    {
        public string b64_json { get; set; }
    }
}
