using Housing.Application.Common.Interface.Persistence;
using DomainEntities = Housing.Domain.Entities;

namespace Housing.Infrastructure.Persistence
{
    public class HousingRepository : BaseRepository<DomainEntities.House>, IHousingRepository<DomainEntities.House>
    {
        public HousingRepository(HousingDbContext housingDbContext) : base(housingDbContext) { }

        public IQueryable<DomainEntities.House> GetAllHouses()
        {
            return _housingDbContext.Houses.AsQueryable();
        }
    }
}
