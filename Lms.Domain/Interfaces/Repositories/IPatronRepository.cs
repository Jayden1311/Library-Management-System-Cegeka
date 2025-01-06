using Lms.Domain.Entities;

namespace Lms.Domain.Interfaces.Repositories;

public interface IPatronRepository
{
    Task AddAsync(Patron patron);
    Task DeleteAsync(Patron patron);
    Task EditAsync(Patron patron);
    Task<Patron?> GetByIdAsync(int id);
    Task<List<Patron>> GetPatronsAsync();
    Task SaveChangesAsync();
}