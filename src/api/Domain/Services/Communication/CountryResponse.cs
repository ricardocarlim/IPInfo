using api.Domain.Models;

namespace api.Domain.Services.Communication
{
    public class CountryResponse : BaseResponse
    {
        public Country Country { get; private set; }
        private CountryResponse(bool success, string message, Country country) : base(success, message)
        {
            Country = country;
        }
      
        public CountryResponse(Country country) : this(true, string.Empty, country)
        { }
      
        public CountryResponse(string message) : this(false, message, null)
        { }
    }
}
