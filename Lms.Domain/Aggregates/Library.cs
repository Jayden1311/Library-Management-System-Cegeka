using System.Linq;
using Lms.Domain.Entities;

namespace Lms.Domain.Aggregates;

public class Library
{
    public int Id { get; private set; } // EF requires a primary key
    public string Name { get; private set; }
    public ICollection<Book> Books { get; private set; } = new List<Book>();

    // Need a private & parameterless constructor for EF
    private Library()
    {
    }

    public Library(string name)
    {
        Name = name;
        Books = new List<Book>();
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    // Should be changed when multiple book copies are allowed in the library with the same ISBN
    public void AddBook(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book), "Book cannot be null.");
        }

        if (Books.Any(b => b.ISBN == book.ISBN))
        {
            throw new InvalidOperationException("Book with the same ISBN already exists in the library.");
        }

        Books.Add(book);
    }

    public void RemoveBook(string isbn)
    {
        var book = Books.FirstOrDefault(b => b.ISBN == isbn);
        if (book == null)
        {
            throw new InvalidOperationException("Book does not exist in the library.");
        }

        Books.Remove(book);
    }

    public IEnumerable<Book> SearchBooks(string keyword)
    {
        return Books.Where(b => b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                b.Genre.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public void CheckoutBook(string isbn, Patron patron)
    {
        var book = Books.FirstOrDefault(b => b.ISBN == isbn);
        if (book == null)
        {
            throw new InvalidOperationException("Book does not exist in the library.");
        }

        if (!book.IsAvailable)
        {
            throw new InvalidOperationException("Book is already checked out.");
        }

        book.MarkAsCheckedOut();
        patron.CheckedOutBooks.Add(book);
    }

    public void ReturnBook(string isbn, Patron patron)
    {
        var book = Books.FirstOrDefault(b => b.ISBN == isbn);
        if (book == null)
        {
            throw new InvalidOperationException("Book does not exist in the library.");
        }

        if (!patron.CheckedOutBooks.Contains(book))
        {
            throw new InvalidOperationException("Book was not checked out by this patron.");
        }

        book.MarkAsReturned();
        patron.CheckedOutBooks.Remove(book);
    }
}   