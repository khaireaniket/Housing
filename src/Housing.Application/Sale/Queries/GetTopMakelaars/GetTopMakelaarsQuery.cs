using Housing.Application.Common.Helpers;
using Housing.Application.Common.Interface.Persistence;
using MediatR;
using DomainEntities = Housing.Domain.Entities;
using MakelaarDTOs = Housing.Application.Sale.Queries.DTOs;

namespace Housing.Application.Sale.Queries.GetTopMakelaars
{
    public record GetTopMakelaarsQuery(int Top, string Location, string Status, bool FilterByTuin) : IRequest<IEnumerable<MakelaarDTOs.Makelaar>>;

    public class GetTopMakelaarsQueryHandler : IRequestHandler<GetTopMakelaarsQuery, IEnumerable<MakelaarDTOs.Makelaar>>
    {
        private readonly IHousingRepository<DomainEntities.House> _housingRepository;

        public GetTopMakelaarsQueryHandler(IHousingRepository<DomainEntities.House> housingRepository)
        {
            _housingRepository = housingRepository;
        }

        public Task<IEnumerable<MakelaarDTOs.Makelaar>> Handle(GetTopMakelaarsQuery request, CancellationToken cancellationToken)
        {
            var topMakelaars = _housingRepository.GetAllHouses()
                                        .Where(w => w.Woonplaats == request.Location && w.VerkoopStatus == request.Status)
                                        .WhereIf(() => request.FilterByTuin == true, w => w.HasTuin == true)
                                        .GroupBy(g => g.MakelaarId)
                                        .Select(proj => new MakelaarDTOs.Makelaar
                                        {
                                            MakelaarId = proj.Key,
                                            MakelaarNaam = proj.First(s => s.MakelaarId == proj.Key).MakelaarNaam,
                                            NumberOfListedHouses = proj.Count()
                                        })
                                        .OrderByDescending(o => o.NumberOfListedHouses)
                                        .Skip(0)
                                        .Take(request.Top)
                                        .AsEnumerable();

            return Task.FromResult(topMakelaars);
        }
    }
}
