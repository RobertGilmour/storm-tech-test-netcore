using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace Todo.Services
{
    public class GravatarService
    {
        private readonly HttpClient _httpClient;

        public GravatarService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string GetUserFullName(string emailAddress)
        {
            var route = $"{Gravatar.GetHash(emailAddress)}.json";
            var response = _httpClient.GetAsync(route).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var profile = JsonConvert.DeserializeObject<GravatarProfile>(content);
                return profile?.Entry?.FirstOrDefault()?.Name.Formatted;
            }
            else
            {
                return null;
            }
        }
    }
}
