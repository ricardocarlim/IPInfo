using app.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace app.Pages.Clientes
{
    public class NovoModel : PageModel
    {
        private readonly IApiCliente _apiClient;

        public NovoModel(IApiCliente apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public ClienteResource Cliente { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)            
                return Page();            

            if (Cliente.imagem != null && Cliente.imagem.Length > 0)            
                Cliente.logotipo = Helper.Image.ConverterParaBase64(Cliente.imagem);            

            var response = await _apiClient.CreateClienteAsync(Cliente);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, errorMessage);
                return Page();
            }

            return RedirectToPage("./Index");
            
        }      
    }
}
