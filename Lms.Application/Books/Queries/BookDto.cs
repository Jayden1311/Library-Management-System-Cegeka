namespace Lms.Application.Books.Queries;

public sealed record BookDto
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public string Genre { get; init; }
    public string ISBN { get; init; }
    public bool IsAvailable { get; init; }
    public int LibraryId { get; init; }
}