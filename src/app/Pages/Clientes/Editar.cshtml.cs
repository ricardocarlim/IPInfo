using app.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace app.Pages.Clientes
{
    public class EditarModel : PageModel
    {
        private readonly IApiCliente _apiClient;
        private readonly IApiLogradouro _apiLogradouro;

        public EditarModel(IApiCliente apiClient, IApiLogradouro apiLogradouro)
        {
            _apiClient = apiClient;
            _apiLogradouro = apiLogradouro;
        }

        [BindProperty]
        public ClienteResource Cliente { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string errors)
        {
            Cliente = await _apiClient.GetClienteAsync(id);
            ModelState.Clear();
            if (!string.IsNullOrEmpty(errors))
            {                
                ModelState.AddModelError(string.Empty, errors);
            }
            if (Cliente == null)
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Cliente.imagem != null && Cliente.imagem.Length > 0)
                Cliente.logotipo = Helper.Image.ConverterParaBase64(Cliente.imagem);

            var response = await _apiClient.UpdateClienteAsync(Cliente.id, Cliente);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();                
                return RedirectToPage(null, new { id = Cliente.id, errors = errorMessage });
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetExcluir(int id, int ClienteId)
        {
            var logradouro = _apiLogradouro.DeleteLogradouroAsync(id);
            return RedirectToPage(null, new { id = ClienteId });
        }
    }
}
