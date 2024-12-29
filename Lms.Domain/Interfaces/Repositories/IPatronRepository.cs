using Lms.Domain.Entities;

namespace Lms.Domain.Interfaces;

public interface IPatronRepository
{
    Task AddAsync(Patron patron);
    Task<Patron> GetByIdAsync(int id);
    Task SaveChangesAsync();
}