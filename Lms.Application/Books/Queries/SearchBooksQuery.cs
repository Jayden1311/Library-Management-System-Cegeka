using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Queries;

public sealed record SearchBooksQuery(string Keyword) : IRequest<List<BookDto>>;

public sealed class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, List<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public SearchBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<BookDto>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.SearchAsync(request.Keyword);
        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Genre = b.Genre,
            ISBN = b.ISBN,
            IsAvailable = b.IsAvailable,
            LibraryId = b.LibraryId
        }).ToList();
    }
}