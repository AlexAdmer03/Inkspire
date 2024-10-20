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

        public ImageGenerationController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateImage([FromBody] ImagePrompt prompt)
        {
            var client = _clientFactory.CreateClient("TogetherAI");
            var request = new HttpRequestMessage(HttpMethod.Post, "inference")
            {
                Content = JsonContent.Create(new
                {
                    model = "black-forest-labs/FLUX.1-schnell",
                    prompt = prompt.Text,
                    width = 1024,
                    height = 768,
                    steps = 3,
                    response_format = "base64"
                })
            };

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ImageGenerationResult>(content);
                return Ok(result);
            }

            return BadRequest("Failed to generate image");
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
