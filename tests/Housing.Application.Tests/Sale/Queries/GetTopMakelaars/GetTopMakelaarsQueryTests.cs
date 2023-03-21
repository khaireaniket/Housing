using Housing.Application.Common.Interface.Persistence;
using Housing.Application.Sale.Queries.GetTopMakelaars;
using Moq;
using Newtonsoft.Json;
using Xunit;
using DomainEntities = Housing.Domain.Entities;

namespace Housing.Application.Tests.Sale.Queries.GetTopMakelaars
{
    public class GetTopMakelaarsQueryTests
    {
        private readonly Mock<IHousingRepository<DomainEntities.House>> _housingRepository;
        private readonly GetTopMakelaarsQueryHandler _getTopMakelaarsQueryHandler;

        public GetTopMakelaarsQueryTests()
        {
            _housingRepository = new Mock<IHousingRepository<DomainEntities.House>>();
            _getTopMakelaarsQueryHandler = new GetTopMakelaarsQueryHandler(_housingRepository.Object);
        }

        [Fact]
        public async Task GetTopMakelaarsQueryHandler_ShouldReturnTop10MakeelarsInAmsterdamForSale_WhenDataIsAvailable()
        {
            // Arrange
            var allHouses = LoadMockData().AsQueryable();
            _housingRepository.Setup(x => x.GetAllHouses()).Returns(allHouses);

            // Act
            var topMakelaars = await _getTopMakelaarsQueryHandler.Handle(new GetTopMakelaarsQuery(10, "Amsterdam", "StatusBeschikbaar", false),
                                                                         CancellationToken.None);

            // Assert
            Assert.NotNull(topMakelaars);
            Assert.True(topMakelaars.Any());
            Assert.True(topMakelaars.Count() == 10);

            var numberOfListedHousesFromFirstMakelaar = topMakelaars.First().NumberOfListedHouses;
            var numberOfListedHousesFromLastMakelaar = topMakelaars.Last().NumberOfListedHouses;

            Assert.True(numberOfListedHousesFromFirstMakelaar > numberOfListedHousesFromLastMakelaar);
        }

        [Fact]
        public async Task GetTopMakelaarsQueryHandler_ShouldReturnTop10MakeelarsInAmsterdamForSaleWithGarden_WhenDataIsAvailable()
        {
            // Arrange
            var allHouses = LoadMockData().AsQueryable();
            _housingRepository.Setup(x => x.GetAllHouses()).Returns(allHouses);

            // Act
            var topMakelaars = await _getTopMakelaarsQueryHandler.Handle(new GetTopMakelaarsQuery(10, "Amsterdam", "StatusBeschikbaar", true),
                                                                         CancellationToken.None);

            // Assert
            Assert.NotNull(topMakelaars);
            Assert.True(topMakelaars.Any());
            Assert.True(topMakelaars.Count() == 10);

            var numberOfListedHousesFromFirstMakelaar = topMakelaars.First().NumberOfListedHouses;
            var numberOfListedHousesFromLastMakelaar = topMakelaars.Last().NumberOfListedHouses;

            Assert.True(numberOfListedHousesFromFirstMakelaar > numberOfListedHousesFromLastMakelaar);
        }

        public List<DomainEntities.House> LoadMockData()
        {
            string mockData = File.ReadAllText(@"MockData\APIMockResponse.json");
            return JsonConvert.DeserializeObject<List<DomainEntities.House>>(mockData)!;
        }
    }
}
