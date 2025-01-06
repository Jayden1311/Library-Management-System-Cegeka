using Lms.Application.Books.Queries;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Library.Queries;

public sealed record GetLibraryByIdQuery(int Id) : IRequest<LibraryDto>;

public sealed class GetLibraryByIdQueryHandler : IRequestHandler<GetLibraryByIdQuery, LibraryDto>
{
    private readonly ILibraryRepository _libraryRepository;

    public GetLibraryByIdQueryHandler(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public async Task<LibraryDto> Handle(GetLibraryByIdQuery request, CancellationToken cancellationToken)
    {
        var library = await _libraryRepository.GetByIdAsync(request.Id);
        if (library == null)
        {
            throw new KeyNotFoundException($"Library with ID {request.Id} not found.");
        }

        return new LibraryDto
        {
            Id = library.Id,
            Name = library.Name,
        };
    }
}