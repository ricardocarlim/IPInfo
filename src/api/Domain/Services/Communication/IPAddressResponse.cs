using api.Domain.Models;

namespace api.Domain.Services.Communication
{
    public class IPAddressResponse : BaseResponse
    {
        public IPAddress IPAddress { get; private set; }
        private IPAddressResponse(bool success, string message, IPAddress ipAddress) : base(success, message)
        {
            IPAddress = ipAddress;
        }
      
        public IPAddressResponse(IPAddress ipAddress) : this(true, string.Empty, ipAddress)
        { }
      
        public IPAddressResponse(string message) : this(false, message, null)
        { }
    }
}
