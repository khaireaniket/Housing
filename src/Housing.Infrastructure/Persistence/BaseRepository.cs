using Housing.Application.Common.Interface.Persistence;
using Housing.Domain.Entities;
using Housing.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Housing.Infrastructure.Persistence
{
    public class BaseRepository<T> : IRepositoryBase<T> where T : EntityBase
    {
        protected readonly HousingDbContext _housingDbContext;

        public BaseRepository(HousingDbContext housingDbContext)
        {
            _housingDbContext = housingDbContext;
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            _housingDbContext.Set<T>().AddRange(entities);
            return await _housingDbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _housingDbContext.Set<T>().CountAsync();
        }

        public virtual async Task<int> CountTuinAsync()
        {
            return await _housingDbContext.Set<House>().CountAsync(c => c.HasTuin == true);
        }
    }
}
