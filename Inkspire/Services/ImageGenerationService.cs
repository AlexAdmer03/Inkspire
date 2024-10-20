using System.Net.Http.Json;
using System.Threading.Tasks;
namespace Inkspire.Services
{
    public interface IImageGenerationService
    {
        Task<string> GenerateImageAsync(string prompt);
    }
    public class ImageGenerationService : IImageGenerationService
    {
        private readonly HttpClient _httpClient;
        public ImageGenerationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GenerateImageAsync(string prompt)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ImageGeneration", new { Text = prompt });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ImageGenerationResult>();
                return result?.b64_json;
            }
            throw new HttpRequestException("Failed to generate image");
        }
    }
    public class ImageGenerationResult
    {
        public string b64_json { get; set; }
    }
}