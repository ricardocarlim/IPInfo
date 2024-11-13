using app.Resources;
using System.Net;
using System.Text;
using System.Web.Http.ModelBinding;

namespace app.Services
{
    public interface IApiCliente
    {
        Task<QueryResultResource<ClienteResource>> GetClientesAsync();
        Task<ClienteResource> GetClienteAsync(int id);
        Task<HttpResponseMessage> CreateClienteAsync(ClienteResource cliente);
        Task<HttpResponseMessage> UpdateClienteAsync(int id, ClienteResource cliente);
        Task DeleteClienteAsync(int id);
    }

    public class ApiCliente : IApiCliente
    {
        public ModelStateDictionary ModelState { get; }
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7177/api";
        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"admin:Pa$$WoRd"));
        public ApiCliente(HttpClient httpClient)
        {
            _httpClient = httpClient;
            ModelState = new ModelStateDictionary();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }

        public async Task<QueryResultResource<ClienteResource>> GetClientesAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/Cliente");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QueryResultResource<ClienteResource>>();
        }

        public async Task<ClienteResource> GetClienteAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/Cliente/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClienteResource>();
        }

        public async Task<HttpResponseMessage> CreateClienteAsync(ClienteResource cliente)
        {
            return await _httpClient.PostAsJsonAsync($"{BaseUrl}/Cliente", cliente);           
        }

        public async Task<HttpResponseMessage> UpdateClienteAsync(int id, ClienteResource cliente)
        {
            return await _httpClient.PutAsJsonAsync($"{BaseUrl}/Cliente/{id}", cliente);
        }

        public async Task DeleteClienteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/Cliente/{id}");
            response.EnsureSuccessStatusCode();
        }
    }

}
