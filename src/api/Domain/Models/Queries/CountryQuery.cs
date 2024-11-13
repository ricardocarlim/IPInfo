using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api.Domain.Models.Queries
{
    public class CountryQuery : Query
    {
        //public string? Nome { get; set; }
        //public string? Email { get; set; }

        public CountryQuery(int page, int itemsPerPage): base(page, itemsPerPage)
            //string? nome, string? email, int page, int itemsPerPage) 
        {
            //Nome = nome;
            //Email = email;
        }
    }   
}
