using Bogus;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lms.WebAPI;

public class FakeDataSeeder
{
    private readonly LmsDbContext _context;

    public FakeDataSeeder(LmsDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Books.Any())
        {
            return; // DB has been seeded
        }

        GenerateAndSeedData();
    }

    private void GenerateAndSeedData()
    {
        var libraries = new List<Library>
        {
            new Library("Central Library"),
            new Library("Westside Library"),
            new Library("Eastside Library")
        };

        var patrons = new List<Patron>
        {
            new Patron("John Doe"),
            new Patron("Jane Smith"),
            new Patron("Alice Johnson"),
            new Patron("Bob Brown")
        };

        var bookFaker = new Faker<Book>()
            .CustomInstantiator(f => new Book(
                f.Vehicle.Model() + " " + f.Vehicle.Model() + " " + f.Vehicle.Model(),
                f.Name.FullName(),
                f.Lorem.Word(),
                f.Random.Replace("##########"),
                f.PickRandom(libraries)
            ));

        var books = bookFaker.Generate(50);

        _context.Libraries.AddRange(libraries);
        _context.Patrons.AddRange(patrons);
        _context.Books.AddRange(books);
        _context.SaveChanges();

        // Check out some books to patrons
        var random = new Random();
        foreach (var patron in patrons)
        {
            var booksToCheckout = books.Where(b => b.IsAvailable).OrderBy(x => random.Next()).Take(5).ToList();
            foreach (var book in booksToCheckout)
            {
                if (book.IsAvailable)
                {
                    patron.CheckoutBook(book);
                }
            }
        }

        _context.SaveChanges();
    }
}