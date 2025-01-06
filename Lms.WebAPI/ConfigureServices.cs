using FluentValidation;
using Lms.Application.Books.Commands.CheckinBook;
using Lms.Application.Books.Commands.CheckoutBook;
using Lms.Application.Books.Commands.CreateBook;
using Lms.Application.Books.Commands.DeleteBook;
using Lms.Application.Books.Commands.EditBook;
using Lms.Application.Library.Commands;
using Lms.Application.Library.Commands.CreateLibrary;
using Lms.Application.Library.Commands.DeleteLibrary;
using Lms.Application.Library.Commands.EditLibrary;
using Lms.Application.Library.Queries;
using Lms.Application.Patron.Commands;
using Lms.Application.Patron.Commands.CreatePatronCommand;
using Lms.Application.Patron.Commands.DeletePatron;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Lms.Infrastructure.Persistence;
using Lms.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Lms.WebAPI
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<LmsDbContext>(options =>
                options.UseInMemoryDatabase("LmsDb"));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetLibrariesQuery).Assembly));
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "LMS API", Version = "v1" }); });
            services.AddOpenApiDocument(configure => { configure.Title = "LMS API"; });

            // Register the Repository services
            services.AddTransient<ILibraryRepository, LibraryRepository>();
            services.AddTransient<IPatronRepository, PatronRepository>();
            services.AddTransient<IBookRepository, BookRepository>();

            // Register Library validators
            services.AddTransient<IValidator<CreateLibraryCommand>, CreateLibraryCommandValidator>();
            services.AddTransient<IValidator<EditLibraryCommand>, EditLibraryCommandValidator>();
            services.AddTransient<IValidator<DeleteLibraryCommand>, DeleteLibraryCommandValidator>();

            // Register Patron validators
            services.AddTransient<IValidator<CreatePatronCommand>, CreatePatronCommandValidator>();
            services.AddTransient<IValidator<EditPatronCommand>, EditPatronCommandValidator>();
            services.AddTransient<IValidator<DeletePatronCommand>, DeletePatronCommandValidator>();

            // Register Book validators
            services.AddTransient<IValidator<CreateBookCommand>, CreateBookCommandValidator>();
            services.AddTransient<IValidator<EditBookCommand>, EditBookCommandValidator>();
            services.AddTransient<IValidator<DeleteBookCommand>, DeleteBookCommandValidator>();
            services.AddTransient<IValidator<CheckinBookCommand>, CheckinBookCommandValidator>();
            services.AddTransient<IValidator<CheckoutBookCommand>, CheckoutBookCommandValidator>();
        }
    }
}