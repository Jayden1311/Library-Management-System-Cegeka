using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Queries;

public sealed record GetBookByIsbnQuery(string ISBN) : IRequest<BookDto>;

public sealed class GetBookByIsbnQueryHandler : IRequestHandler<GetBookByIsbnQuery, BookDto>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIsbnQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto> Handle(GetBookByIsbnQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByISBNAsync(request.ISBN);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ISBN {request.ISBN} not found.");
        }

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            ISBN = book.ISBN,
            IsAvailable = book.IsAvailable,
            LibraryId = book.LibraryId
        };
    }
}