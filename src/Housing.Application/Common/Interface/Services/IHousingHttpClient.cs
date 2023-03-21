namespace Housing.Application.Common.Interface.Services
{
    public interface IHousingHttpClient
    {
        Task<string?> FetchHousesBySearchQueryAndPageNumber(int pageNumber, string searchQuery);

        Task<HttpResponseMessage> FetchHousesBySearchQueryAndPageNumberAsync(int pageNumber, string searchQuery);
    }
}
