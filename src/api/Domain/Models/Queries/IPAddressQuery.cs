using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api.Domain.Models.Queries
{
    public class IPAddressQuery : Query
    {
        public string? IP{ get; set; }
        
        public IPAddressQuery() : base(0, 10) { }

        public IPAddressQuery(string ip, int page, int itemsPerPage): base(page, itemsPerPage)            
        {
            IP = ip;
        }
    }   
}
