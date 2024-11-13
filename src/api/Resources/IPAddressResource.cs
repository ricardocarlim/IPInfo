using api.Domain.Models;

namespace api.Resources
{
    public class IPAddressResource
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public string IP { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
