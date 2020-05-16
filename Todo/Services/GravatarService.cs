using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        public async Task<string> GetUserFullName(string emailAddress)
        {
            var route = $"{Gravatar.GetHash(emailAddress)}.json";
            var response = await _httpClient.GetAsync(route);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
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
