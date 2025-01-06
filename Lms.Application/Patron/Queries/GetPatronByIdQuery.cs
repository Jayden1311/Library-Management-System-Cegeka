using Lms.Application.Books.Queries;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Patron.Queries;

public sealed record GetPatronByIdQuery(int Id) : IRequest<PatronDto>;

public sealed class GetPatronByIdQueryHandler : IRequestHandler<GetPatronByIdQuery, PatronDto>
{
    private readonly IPatronRepository _patronRepository;

    public GetPatronByIdQueryHandler(IPatronRepository patronRepository)
    {
        _patronRepository = patronRepository;
    }

    public async Task<PatronDto> Handle(GetPatronByIdQuery request, CancellationToken cancellationToken)
    {
        var patron = await _patronRepository.GetByIdAsync(request.Id);
        if (patron == null)
        {
            throw new KeyNotFoundException($"Patron with ID {request.Id} not found.");
        }

        return new PatronDto
        {
            Id = patron.Id,
            Name = patron.Name,
            CheckedOutBooks = patron.CheckedOutBooks.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author
            }).ToList()
        };
    }
}