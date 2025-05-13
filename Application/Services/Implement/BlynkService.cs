using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class BlynkService : IBlynkService
    {
        private readonly HttpClient _httpClient;
        private const string BlynkToken = "xRd0sDuPYqFjPI1ZSHRr7Bd1cJq3fH2Y"; 
        private const string BlynkBaseUrl = "https://sgp1.blynk.cloud"; 

        public BlynkService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, object?>> GetAllDatastreamValuesAsync()
        {
            try
            {
                var url = $"{BlynkBaseUrl}/external/api/getAll?token={BlynkToken}";
                Console.WriteLine($"Fetching all Datastreams from URL: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching all datastreams: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                // Chuyển đổi JsonElement sang object
                var result = new Dictionary<string, object?>();
                foreach (var kvp in data)
                {
                    result[kvp.Key] = ConvertJsonElementToString(kvp.Value);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        // Hàm chuyển đổi JsonElement sang chuỗi
        private string? ConvertJsonElementToString(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.ToString(), 
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null, 
            };
        }
    }
}
