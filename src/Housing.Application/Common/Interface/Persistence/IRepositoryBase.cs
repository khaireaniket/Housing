using Housing.Domain.Entities;
using Housing.Domain.Entities.Base;

namespace Housing.Application.Common.Interface.Persistence
{
    public interface IRepositoryBase<T> where T : EntityBase
    {
        Task<bool> AddRangeAsync(IEnumerable<T> entities);

        Task<int> CountAsync();

        Task<int> CountTuinAsync();
    }
}
