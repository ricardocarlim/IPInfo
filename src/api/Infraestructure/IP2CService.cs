using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace api.Infraestructure
{
    public static class IP2CService
    {
        private const string ApiUrlTemplate = "http://ip2c.org/{0}";

        public static async Task<Domain.Models.Country> GetCountryInfoFromIPAsync(string ip)
        {
            try
            {
                var result = await GetApiResponseAsync(ip);

                if (string.IsNullOrEmpty(result))
                    return null;

                return ParseCountryInfo(result);
            }
            catch (WebException)
            {
                return null;
            }
        }

        private static async Task<string> GetApiResponseAsync(string ip)
        {
            string url = string.Format(ApiUrlTemplate, ip);
            HttpWebRequest request = CreateRequest(url);

            using (WebResponse response = await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static HttpWebRequest CreateRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            return request;
        }

        private static Domain.Models.Country ParseCountryInfo(string result)
        {
            if (result[0] == '1')
            {
                string[] reply = result.Split(';');
                return new Domain.Models.Country
                {
                    ThreeLetterCode = reply[2],
                    TwoLetterCode = reply[1],
                    Name = reply[3],
                    CreatedAt = DateTime.Now
                };
            }

            return null;
        }
    }
}
