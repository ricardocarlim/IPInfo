using app.Resources;
using System.Text;

namespace app.Services
{
    public interface IApiLogradouro
    {
        Task CreateLogradouroAsync(LogradouroResource Logradouro);
        Task UpdateLogradouroAsync(int id, LogradouroResource Logradouro);
        Task DeleteLogradouroAsync(int id);
    }

    public class ApiLogradouro : IApiLogradouro
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7177/api";
        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"admin:Pa$$WoRd"));

        public ApiLogradouro(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }
        
        public async Task CreateLogradouroAsync(LogradouroResource Logradouro)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Logradouro", Logradouro);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateLogradouroAsync(int id, LogradouroResource Logradouro)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/Logradouro/{id}", Logradouro);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteLogradouroAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/Logradouro/{id}");
            response.EnsureSuccessStatusCode();
        }
    }

}
