using Lms.Application.Books.Queries;

namespace Lms.Application.Library.Queries;

public sealed record LibraryDto
{
    public int Id { get; init; }
    public string Name { get; init; }
}