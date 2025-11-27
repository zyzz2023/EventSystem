using EventProcessor.Infrastructure.Data;
using EventProcessor.Models;
using Microsoft.EntityFrameworkCore; 

namespace EventProcessor.Infrastructure.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public IncidentRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Incident?> GetByIdAsync(Guid id)
        {

            using var context = _contextFactory.CreateDbContext();
            return await context.Incidents
                .AsNoTracking()
                .Include(i => i.Events)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Incident>> GetIncidentsAsync(int page = 1, int pageSize = 20)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Incidents
                .AsNoTracking()
                .Include(i => i.Events)
                .OrderByDescending(i => i.Time)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Incident incident)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Incidents.AddAsync(incident);
            await context.SaveChangesAsync();
        }
    }
}
