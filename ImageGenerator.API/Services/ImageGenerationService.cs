using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class ImageGenerationService
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
            return result.B64_json;
        }

        throw new HttpRequestException("Failed to generate image");
    }
}

public class ImageGenerationResult
{
    public string B64_json { get; set; }
}