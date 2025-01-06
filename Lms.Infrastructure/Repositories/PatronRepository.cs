using Lms.Domain.Entities;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Infrastructure.Repositories
{
    public class PatronRepository : IPatronRepository
    {
        private readonly LmsDbContext _context;
        private readonly ILogger<PatronRepository> _logger;

        public PatronRepository(LmsDbContext context, ILogger<PatronRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Patron patron)
        {
            _logger.LogInformation("Adding a new patron with ID: {Id}", patron.Id);
            await _context.Patrons.AddAsync(patron);
        }

        public Task DeleteAsync(Patron patron)
        {
            _logger.LogInformation("Deleting patron with ID: {Id}", patron.Id);

            if (patron == null)
            {
                _logger.LogWarning("Patron is null");
                throw new ArgumentNullException(nameof(patron));
            }

            _context.Patrons.Remove(patron);
            return Task.CompletedTask;
        }

        public Task EditAsync(Patron patron)
        {
            _logger.LogInformation("Editing patron with ID: {Id}", patron.Id);

            if (patron == null)
            {
                _logger.LogWarning("Patron is null");
                throw new ArgumentNullException(nameof(patron));
            }

            _context.Patrons.Update(patron);
            return Task.CompletedTask;
        }

        public async Task<Patron?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching patron with ID: {ID}", id);
            var patron = await _context.Patrons.FindAsync(id);

            if (patron == null)
            {
                _logger.LogWarning("Patron with ID {ID} not found", id);
                throw new KeyNotFoundException($"Patron with ID {id} not found");
            }

            return patron;
        }

        public async Task<List<Patron>> GetPatronsAsync()
        {
            _logger.LogInformation("Fetching all patrons");
            return await _context.Patrons.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving changes to the database");
            await _context.SaveChangesAsync();
        }
    }
}