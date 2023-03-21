using Housing.Application.Common.Interface.Services;
using System.Net;

namespace Housing.Infrastructure.Services
{
    public class HousingHttpClient : IHousingHttpClient
    {
        private readonly HttpClient _httpClient;

        public HousingHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> FetchHousesBySearchQueryAndPageNumber(int pageNumber, string searchQuery)
        {
            var response = await _httpClient.GetAsync($"feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo={searchQuery}&page={pageNumber}&pagesize=25");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public Task<HttpResponseMessage> FetchHousesBySearchQueryAndPageNumberAsync(int pageNumber, string searchQuery)
        {
            return _httpClient.GetAsync($"feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo={searchQuery}&page={pageNumber}&pagesize=25");
        }
    }
}
