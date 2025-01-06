using Lms.Application.Books.Queries;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Library.Queries;

public sealed record GetLibrariesQuery : IRequest<List<LibraryDto>>;

public sealed class GetLibrariesQueryHandler : IRequestHandler<GetLibrariesQuery, List<LibraryDto>>
{
    private readonly ILibraryRepository _libraryRepository;

    public GetLibrariesQueryHandler(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public async Task<List<LibraryDto>> Handle(GetLibrariesQuery request, CancellationToken cancellationToken)
    {
        var libraries = await _libraryRepository.GetAllAsync();
        return libraries.Select(l => new LibraryDto
        {
            Id = l.Id,
            Name = l.Name,
        }).ToList();
    }
}