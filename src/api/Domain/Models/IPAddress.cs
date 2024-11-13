namespace api.Domain.Models
{
    public class IPAddress
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public string IP { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
