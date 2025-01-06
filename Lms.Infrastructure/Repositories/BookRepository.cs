using Lms.Domain.Entities;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LmsDbContext _context;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(LmsDbContext context, ILogger<BookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Book book)
        {
            _logger.LogInformation("Adding a new book with ISBN: {Isbn}", book.ISBN);
            await _context.Books.AddAsync(book);
        }

        public Task DeleteAsync(Book book)
        {
            _logger.LogInformation("Deleting book with ID: {Id}", book.Id);

            if (book == null)
            {
                _logger.LogError("Book is null");
                throw new System.ArgumentNullException(nameof(book));
            }

            _context.Books.Remove(book);
            return Task.CompletedTask;
        }

        public Task EditAsync(Book book)
        {
            _logger.LogInformation("Editing book with ID: {Id}", book.Id);

            if (book == null)
            {
                _logger.LogError("Book is null");
                throw new System.ArgumentNullException(nameof(book));
            }

            _context.Books.Update(book);
            return Task.CompletedTask;
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching book with ID: {ID}", id);
            var book = await _context.Books.Include(b => b.Library).FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                _logger.LogError("Book with ID {ID} not found", id);
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }

            return book;
        }

        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            _logger.LogInformation("Fetching book with ISBN: {Isbn}", isbn);
            var book = await _context.Books.Include(b => b.Library).FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                _logger.LogError("Book with ISBN {Isbn} not found", isbn);
                throw new KeyNotFoundException($"Book with ISBN {isbn} not found.");
            }

            return book;
        }

        public async Task<IEnumerable<Book>> SearchAsync(string keyword)
        {
            _logger.LogInformation("Searching for books with keyword: {Keyword}", keyword);
            return await _context.Books
                .Include(b => b.Library)
                .Where(b => b.Title.Contains(keyword) || b.Author.Contains(keyword))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all books");
            return await _context.Books.Include(b => b.Library).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving changes to the database");
            await _context.SaveChangesAsync();
        }
    }
}