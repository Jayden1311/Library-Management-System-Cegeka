# Library-Application-Cegeka

This application is a library management system developed as a test for Cegeka. It allows users to manage libraries, books, and patrons. The application is built using C# and leverages Entity Framework Core for data persistence.

## Features

- **Library Management**: Add, remove, and search for libraries.
- **Book Management**: Add, remove, and search for books within libraries.
- **Patron Management**: Add patrons and manage book checkouts and returns.

## Project Structure

- `Lms.Domain`: Contains the domain entities and interfaces.
- `Lms.Infrastructure`: Contains the implementation of repositories and the `LibraryDbContext`.
- `Lms.Domain.Tests`: Contains unit tests for the domain layer.
- `Lms.Infrastructure.Tests`: Contains unit tests for the infrastructure layer.
- `Lms.Domain.IntegrationTests`: Contains integration tests for the application.

## Testing

To test the application, please refer to the unit tests provided in the `Lms.Domain.Tests` and `Lms.Infrastructure.Tests` projects. These tests cover various scenarios and ensure the correctness of the application's functionality.

## Dependencies

The application uses the following NuGet packages:

- `FluentAssertions`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.InMemory`
- `Microsoft.NET.Test.Sdk`
- `Moq`
- `xunit`

## Running Tests

To run the tests, you can use the following command in the terminal:

```sh
dotnet test
```

This command will execute all the unit tests and provide a summary of the test results.

## Conclusion

This library management system is a comprehensive solution for managing libraries, books, and patrons. The unit tests ensure the reliability and correctness of the application.