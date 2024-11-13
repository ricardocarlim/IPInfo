using app.Resources;
using app.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace app.Pages.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly IApiCliente _apiClient;

        public IndexModel(IApiCliente apiClient)
        {
            _apiClient = apiClient;
        }

        public QueryResultResource<ClienteResource> Clientes { get; set; }

        public async Task OnGetAsync()
        {
            Clientes = await _apiClient.GetClientesAsync();
        }

        public IActionResult OnGetExcluir(int id)
        {
            var cliente = _apiClient.DeleteClienteAsync(id);
            
            return RedirectToPage();
        }
    }
}
