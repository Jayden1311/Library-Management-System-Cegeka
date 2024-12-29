using Lms.Domain.Aggregates;

namespace Lms.Domain.Entities;

public class Book
{
    public int Id { get; private set; } // EF requires a primary key
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Genre { get; private set; }
    public string ISBN { get; private set; }
    public bool IsAvailable { get; private set; }
    public int LibraryId { get; private set; }
    public Library Library { get; private set; }

    // Need a private & parameterless constructor for EF
    // ReSharper disable once UnusedMember.Local
    private Book()
    {
    }

    public Book(string title, string author, string genre, string isbn, Library library)
    {
        Title = title;
        Author = author;
        Genre = genre;
        ISBN = isbn;
        IsAvailable = true;
        Library = library;
        LibraryId = library.Id;
    }

    public void UpdateProperties(string title, string author, string genre)
    {
        Title = title;
        Author = author;
        Genre = genre;
    }

    public void UpdateISBN(string isbn)
    {
        ISBN = isbn;
    }

    public void MarkAsCheckedOut()
    {
        if (!IsAvailable)
            throw new InvalidOperationException("Book is already checked out.");
        IsAvailable = false;
    }

    public void MarkAsReturned()
    {
        if (IsAvailable)
            throw new InvalidOperationException("Book is not checked out.");
        IsAvailable = true;
    }
}