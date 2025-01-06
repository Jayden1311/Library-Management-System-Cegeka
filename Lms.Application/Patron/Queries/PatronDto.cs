using Lms.Application.Books.Queries;

namespace Lms.Application.Patron.Queries;

public sealed record PatronDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public ICollection<BookDto> CheckedOutBooks { get; init; }
}