using api.Domain.Models;

namespace api.Resources
{
    public class CountryResource
    {       
        public string Name { get; set; }
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }     
    }
}
