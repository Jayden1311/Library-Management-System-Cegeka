using FluentValidation;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Commands.DeleteBook;

public sealed record DeleteBookCommand(int Id) : IRequest;

public sealed class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IValidator<DeleteBookCommand> _validator;

    public DeleteBookCommandHandler(IBookRepository bookRepository, IValidator<DeleteBookCommand> validator)
    {
        _bookRepository = bookRepository;
        _validator = validator;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var book = await _bookRepository.GetByIdAsync(request.Id);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {request.Id} not found.");
        }

        await _bookRepository.DeleteAsync(book);
        await _bookRepository.SaveChangesAsync();
    }
}