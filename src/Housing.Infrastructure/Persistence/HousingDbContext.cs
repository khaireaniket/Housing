using Microsoft.EntityFrameworkCore;
using DomainEntities = Housing.Domain.Entities;

namespace Housing.Infrastructure.Persistence
{
    public class HousingDbContext : DbContext
    {
        public HousingDbContext(DbContextOptions<HousingDbContext> options) : base(options) { }

        public DbSet<DomainEntities.House> Houses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DomainEntities.House>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
