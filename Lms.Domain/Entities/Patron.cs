using Lms.Domain.Aggregates;

namespace Lms.Domain.Entities
{
    public class Patron
    {
        public int Id { get; private set; } // EF requires a primary key
        public string Name { get; private set; }
        public List<Book> CheckedOutBooks { get; private set; } = new();

        public Patron(string name)
        {
            Name = name;
        }

        // Parameterless constructor
        public Patron()
        {
        }

        public void UpdateName(string name)
        {
            Name = name;
        }


        public void ReturnBook(Book book)
        {
            if (!CheckedOutBooks.Contains(book))
                throw new InvalidOperationException("This book was not checked out by this patron.");
            book.MarkAsReturned();
            CheckedOutBooks.Remove(book);
        }

        public void CheckoutBook(Book book)
        {
            book.MarkAsCheckedOut();
            CheckedOutBooks.Add(book);
        }

        public void CheckoutBook(string isbn, Library library)
        {
            var book = library.Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null)
                throw new InvalidOperationException("Book does not exist in the library.");
            if (!book.IsAvailable)
                throw new InvalidOperationException("Book is already checked out.");

            book.MarkAsCheckedOut();
            CheckedOutBooks.Add(book);
        }
    }
}