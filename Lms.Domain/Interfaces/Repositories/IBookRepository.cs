using Lms.Domain.Entities;

namespace Lms.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task AddAsync(Book book);
        Task<Book> GetByIdAsync(int id);
        Task<Book> GetByISBNAsync(string isbn);
        Task<IEnumerable<Book>> SearchAsync(string keyword);
        Task SaveChangesAsync();
    }
}