using Lms.Application.Books.Queries;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Patron.Queries;

public sealed record GetPatronsQuery : IRequest<List<PatronDto>>;

public sealed class GetPatronsQueryHandler : IRequestHandler<GetPatronsQuery, List<PatronDto>>
{
    private readonly IPatronRepository _patronRepository;

    public GetPatronsQueryHandler(IPatronRepository patronRepository)
    {
        _patronRepository = patronRepository;
    }

    public async Task<List<PatronDto>> Handle(GetPatronsQuery request, CancellationToken cancellationToken)
    {
        var patrons = await _patronRepository.GetPatronsAsync();
        return patrons.Select(patron => new PatronDto
        {
            Id = patron.Id,
            Name = patron.Name,
            CheckedOutBooks = patron.CheckedOutBooks.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author
            }).ToList()
        }).ToList();
    }
}