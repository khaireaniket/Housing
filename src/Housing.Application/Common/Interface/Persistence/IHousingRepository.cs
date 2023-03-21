using Housing.Domain.Entities.Base;
using DomainEntities = Housing.Domain.Entities;

namespace Housing.Application.Common.Interface.Persistence
{
    public interface IHousingRepository<T> : IRepositoryBase<T> where T : EntityBase
    {
        IQueryable<DomainEntities.House> GetAllHouses();
    }
}
