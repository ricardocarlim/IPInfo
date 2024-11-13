using System.Net;

namespace api.Infraestructure
{
    public static class IP2CService
    {
        public static async Task<Domain.Models.Country> GetCountryInfoFromIPAsync(string ip)
        {
            var country = new Domain.Models.Country();
            string url = $"http://ip2c.org/{ip}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string result = await reader.ReadToEndAsync();

                    switch (result[0])
                    {
                        case '0':
                            return null;                            
                        case '1':
                            string[] reply = result.Split(';');                            
                            country.ThreeLetterCode = reply[2];
                            country.TwoLetterCode = reply[1];
                            country.Name = reply[3];
                            country.CreatedAt = DateTime.Now;

                            break;
                        case '2':
                            return null;                            
                        default:
                            return null;                            
                    }
                }
            }
            catch (WebException ex)
            {
                return null;
            }

            return country;
        }
    }
}
