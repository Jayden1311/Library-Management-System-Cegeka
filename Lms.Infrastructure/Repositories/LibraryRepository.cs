﻿using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces;
using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Infrastructure.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<LibraryRepository> _logger;

        public LibraryRepository(LibraryDbContext context, ILogger<LibraryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Library library)
        {
            _logger.LogInformation("Adding a new library with ID: {Id}", library.Id);
            await _context.Libraries.AddAsync(library);
        }

        public async Task<Library> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching library with ID: {ID}", id);
            var library = await _context.Libraries.FindAsync(id);

            if (library == null)
            {
                _logger.LogError("Library with ID {ID} not found", id);
                throw new KeyNotFoundException($"Library with ID {id} not found.");
            }

            return library;
        }

        public async Task<IEnumerable<Library>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all libraries");
            return await _context.Libraries.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving changes to the database");
            await _context.SaveChangesAsync();
        }
    }
}