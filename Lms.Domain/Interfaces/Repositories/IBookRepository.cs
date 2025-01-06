using Lms.Domain.Entities;

namespace Lms.Domain.Interfaces.Repositories
{
    public interface IBookRepository
    {
        Task AddAsync(Book book);
        Task DeleteAsync(Book book);
        Task EditAsync(Book book);
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByISBNAsync(string isbn);
        Task<IEnumerable<Book>> SearchAsync(string keyword);
        Task<IEnumerable<Book>> GetAllAsync();
        Task SaveChangesAsync();
    }
}