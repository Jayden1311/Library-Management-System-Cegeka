using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Queries;

public sealed record GetBooksQuery : IRequest<List<BookDto>>;

public sealed class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, List<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(book => new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            ISBN = book.ISBN,
            IsAvailable = book.IsAvailable,
            LibraryId = book.LibraryId
        }).ToList();
    }
}