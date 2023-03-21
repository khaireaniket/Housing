using Housing.Application.Common.Interface.Persistence;
using Housing.Application.Common.Interface.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DomainEntities = Housing.Domain.Entities;

namespace Housing.Infrastructure.Services
{
    public class HousingScraper : BackgroundService, IHousingScraper
    {
        private readonly ILogger<HousingScraper> _logger;
        private readonly IHousingHttpClient _housingHttpClient;
        private readonly IServiceProvider _services;
        private readonly IHousingRepository<DomainEntities.House> _housingRepository;

        public HousingScraper(ILogger<HousingScraper> logger, IHousingHttpClient housingHttpClient,
                              IServiceProvider services, IHousingRepository<DomainEntities.House> housingRepository)
        {
            _logger = logger;
            _housingHttpClient = housingHttpClient;
            _services = services;
            _housingRepository = housingRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scraping of Housing API has started");

            while (true)
            {
                var (lastPageScrapedByLocation, lastPageScrapedByLocationAndTuin) = await EvaluateLastPageScraped();

                var houseArray = new JArray();

                var (listHttpResponseMessageByLocation, listHttpResponseMessageByLocationAndTuin, totalNumberOfPagesByLocation, totalNumberOfPagesByLocationAndTuin)
                = await APIScraping(lastPageScrapedByLocation, lastPageScrapedByLocationAndTuin);

                var httpResponseMessagesByLocation = await Task.WhenAll(listHttpResponseMessageByLocation);
                var httpResponseMessagesByLocationAndTuin = await Task.WhenAll(listHttpResponseMessageByLocationAndTuin);

                await PopulateHousesByLocation(houseArray, httpResponseMessagesByLocation);

                AddTuinProperty(houseArray);

                await PopulateHousesByLocationAndTuin(houseArray, httpResponseMessagesByLocationAndTuin);

                try
                {
                    await InsertHouses(houseArray);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    Console.WriteLine(ex);

                    // Implement circuit breaker pattern
                }

                var housesScraped = await _housingRepository.CountAsync();

                if (lastPageScrapedByLocation >= totalNumberOfPagesByLocation && lastPageScrapedByLocationAndTuin <= totalNumberOfPagesByLocationAndTuin)
                {
                    _logger.LogInformation("Scraping of Housing API has completed successfully");
                    _logger.LogInformation($"{housesScraped} houses extracted");
                    break;
                }
                else
                {
                    _logger.LogInformation("Scraping of Housing API will resume after 5 seconds");

                    // Implement circuit breaker pattern
                    await Task.Delay(5000);
                }
            }
        }

        private async Task<(int, int)> EvaluateLastPageScraped()
        {
            var lastPageScrapedByLocation = 0;
            var lastPageScrapedByLocationAndTuin = 0;

            var numberOfHousesInserted = await _housingRepository.CountAsync();
            var numberOfHousesWithTuinInserted = await _housingRepository.CountTuinAsync();

            if (numberOfHousesInserted > 0)
            {
                lastPageScrapedByLocation = Convert.ToInt32(Math.Round(numberOfHousesInserted / 25d, MidpointRounding.ToZero));
            }

            if (numberOfHousesWithTuinInserted > 0)
            {
                lastPageScrapedByLocationAndTuin = Convert.ToInt32(Math.Round(numberOfHousesWithTuinInserted / 25d, MidpointRounding.ToZero));
            }

            return (lastPageScrapedByLocation, lastPageScrapedByLocationAndTuin);
        }

        private async Task<(List<Task<HttpResponseMessage>>, List<Task<HttpResponseMessage>>, int, int)> APIScraping(int lastPageScrapedByLocation, int lastPageScrapedByLocationAndTuin)
        {
            var listHttpResponseMessageByLocation = new List<Task<HttpResponseMessage>>();
            var listHttpResponseMessageByLocationAndTuin = new List<Task<HttpResponseMessage>>();

            var totalNumberOfPagesByLocation = await GetTotalNumberOfPagesByLocation();
            var totalNumberOfPagesByLocationAndTuin = await GetTotalNumberOfPagesByLocationAndTuin();

            do
            {
                try
                {
                    var fetchedHousesByLocation = _housingHttpClient.FetchHousesBySearchQueryAndPageNumberAsync(lastPageScrapedByLocation, "/amsterdam/");
                    listHttpResponseMessageByLocation.Add(fetchedHousesByLocation);

                    if (lastPageScrapedByLocationAndTuin <= totalNumberOfPagesByLocationAndTuin)
                    {
                        var fetchedHousesByLocationAndTuin = _housingHttpClient.FetchHousesBySearchQueryAndPageNumberAsync(lastPageScrapedByLocationAndTuin, "/amsterdam/tuin/");
                        listHttpResponseMessageByLocationAndTuin.Add(fetchedHousesByLocationAndTuin);
                    }

                    lastPageScrapedByLocation++;
                    lastPageScrapedByLocationAndTuin++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    _logger.LogError(ex.Message, ex);

                    // Implement circuit breaker pattern
                    continue;
                }
            }
            while (lastPageScrapedByLocation <= totalNumberOfPagesByLocation);

            return (listHttpResponseMessageByLocation, listHttpResponseMessageByLocationAndTuin, totalNumberOfPagesByLocation, totalNumberOfPagesByLocationAndTuin);
        }

        private async Task PopulateHousesByLocation(JArray houseArray, HttpResponseMessage[] httpResponseMessagesByLocation)
        {
            foreach (var message in httpResponseMessagesByLocation)
            {
                var housesResponse = await message.Content.ReadAsStringAsync();
                if (housesResponse is not null)
                {
                    var houseObject = JsonConvert.DeserializeObject<JObject>(housesResponse);

                    if (houseObject is not null)
                    {
                        if (houseObject.ContainsKey("Objects"))
                        {
                            houseArray.Merge(houseObject.Value<JArray>("Objects")!);
                        }
                    }
                }
            }
        }

        private void AddTuinProperty(JArray houseArray)
        {
            foreach (var house in houseArray)
            {
                house["HasTuin"] = false;
            }
        }

        private async Task PopulateHousesByLocationAndTuin(JArray houseArray, HttpResponseMessage[] httpResponseMessagesByLocationAndTuin)
        {
            foreach (var message in httpResponseMessagesByLocationAndTuin)
            {
                var housesResponse = await message.Content.ReadAsStringAsync();
                if (housesResponse is not null)
                {
                    var houseObject = JsonConvert.DeserializeObject<JObject>(housesResponse);

                    if (houseObject is not null)
                    {
                        if (houseObject.ContainsKey("Objects"))
                        {
                            var housesWithTuin = houseObject.Value<JArray>("Objects")!;

                            foreach (var house in housesWithTuin)
                            {
                                var houseId = house.SelectToken("Id", false)!.Value<string>();

                                if (houseArray.Any(a => a.Value<string>("Id")!.Equals(houseId)))
                                {
                                    houseArray.First(a => a.Value<string>("Id")!.Equals(houseId))["HasTuin"] = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task<int> GetTotalNumberOfPagesByLocation()
        {
            var firstHousesResponseByLocation = await _housingHttpClient.FetchHousesBySearchQueryAndPageNumber(0, "/amsterdam/");
            if (firstHousesResponseByLocation is not null)
            {
                var firstHouseByLocationObject = JsonConvert.DeserializeObject<JObject>(firstHousesResponseByLocation);

                if (firstHouseByLocationObject is not null)
                {
                    if (firstHouseByLocationObject.SelectToken("Paging.AantalPaginas", false) is not null)
                    {
                        return int.Parse(firstHouseByLocationObject.SelectToken("Paging.AantalPaginas", false)!.ToString());
                    }
                }
            }

            return 0;
        }

        private async Task<int> GetTotalNumberOfPagesByLocationAndTuin()
        {
            var firstHousesResponseByLocationAndTuin = await _housingHttpClient.FetchHousesBySearchQueryAndPageNumber(0, "/amsterdam/tuin/");
            if (firstHousesResponseByLocationAndTuin is not null)
            {
                var firstHouseByLocationAndTuinObject = JsonConvert.DeserializeObject<JObject>(firstHousesResponseByLocationAndTuin);

                if (firstHouseByLocationAndTuinObject is not null)
                {
                    if (firstHouseByLocationAndTuinObject.SelectToken("Paging.AantalPaginas", false) is not null)
                    {
                        return int.Parse(firstHouseByLocationAndTuinObject.SelectToken("Paging.AantalPaginas", false)!.ToString());
                    }
                }
            }

            return 0;
        }

        private async Task<bool> InsertHouses(JArray houseArray)
        {
            var uniqueHouses = houseArray.GroupBy(c => c.SelectToken("Id")).ToArray();

            var listOfDomainHouses = uniqueHouses.Select(c => new DomainEntities.House(
                                                        houseId: c.First().SelectToken("Id", false)!.Value<string>() ?? string.Empty,
                                                        adres: c.First().SelectToken("Adres", false)!.Value<string>() ?? string.Empty,
                                                        postcode: c.First().SelectToken("Postcode", false)!.Value<string>() ?? string.Empty,
                                                        woonplaats: c.First().SelectToken("Woonplaats", false)!.Value<string>() ?? string.Empty,
                                                        verkoopStatus: c.First().SelectToken("VerkoopStatus", false)!.Value<string>() ?? string.Empty,
                                                        makelaarId: c.First().SelectToken("MakelaarId", false)!.Value<int?>() ?? 0,
                                                        makelaarNaam: c.First().SelectToken("MakelaarNaam", false)!.Value<string>() ?? string.Empty,
                                                        hasTuin: c.First().SelectToken("HasTuin", false)!.Value<bool>()));

            return await _housingRepository.AddRangeAsync(listOfDomainHouses);
        }
    }
}
