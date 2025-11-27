using EventProcessor.Infrastructure.Data;
using EventProcessor.Models;
using Microsoft.EntityFrameworkCore; 

namespace EventProcessor.Infrastructure.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly AppDbContext _context;

        public IncidentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Incident?> GetByIdAsync(Guid id)
        {
            return await _context.Incidents
                .Include(i => i.Events)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Incident>> GetIncidentsAsync(int page = 1, int pageSize = 20)
        {
            return await _context.Incidents
                .Include(i => i.Events)
                .OrderByDescending(i => i.Time)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Incident incident)
        {
            await _context.Incidents.AddAsync(incident);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
