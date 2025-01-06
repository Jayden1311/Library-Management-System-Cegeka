using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Queries;

public sealed record GetBookByIdQuery(int BookId) : IRequest<BookDto>;

public sealed class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {request.BookId} not found.");
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