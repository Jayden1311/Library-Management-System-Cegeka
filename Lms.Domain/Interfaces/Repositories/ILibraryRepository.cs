using Lms.Domain.Aggregates;

namespace Lms.Domain.Interfaces.Repositories
{
    public interface ILibraryRepository
    {
        Task AddAsync(Library library);
        Task DeleteAsync(Library library);
        Task EditAsync(Library library);
        Task<Library?> GetByIdAsync(int id);
        Task<IEnumerable<Library>> GetAllAsync();
        Task SaveChangesAsync();
    }
}