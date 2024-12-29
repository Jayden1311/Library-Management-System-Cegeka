using Lms.Domain.Aggregates;

namespace Lms.Domain.Interfaces
{
    public interface ILibraryRepository
    {
        Task AddAsync(Library library);
        Task<Library> GetByIdAsync(int id);
        Task<IEnumerable<Library>> GetAllAsync();
        Task SaveChangesAsync();
    }
}