using app.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace app.Pages.Logradouro
{
    public class EditarModel : PageModel
    {
        private readonly IApiLogradouro _apiLogradouro;
        public EditarModel(IApiLogradouro apiLogradouro)
        {
            _apiLogradouro = apiLogradouro;
        }

        [BindProperty]
        public LogradouroResource Logradouro{ get; set; }
        public async Task<IActionResult> OnGetAsync([FromQuery]int? id, string? endereco, int ClienteId)
        {
            Logradouro = new LogradouroResource();
            Logradouro.id = id.Value;                                  
            Logradouro.endereco = endereco;
            Logradouro.ClienteId = ClienteId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Logradouro.id == null || Logradouro.id <= 0)            
                await _apiLogradouro.CreateLogradouroAsync(Logradouro);            
            else
                await _apiLogradouro.UpdateLogradouroAsync(Logradouro.id, Logradouro);

            return RedirectToPage("/Clientes/Editar", new { id = Logradouro.ClienteId });
        }
    }
}
