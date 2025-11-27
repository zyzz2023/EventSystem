using EventProcessor.Models;

namespace EventProcessor.Infrastructure.Repositories
{
    public interface IIncidentRepository
    {
        Task<Incident?> GetByIdAsync(Guid id);
        Task<List<Incident>> GetIncidentsAsync(int page = 1, int pageSize = 20);
        Task AddAsync(Incident incident);
        Task SaveChangesAsync();
    }
}
