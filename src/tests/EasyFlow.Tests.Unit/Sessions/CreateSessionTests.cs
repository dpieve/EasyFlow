using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EasyFlow.Tests.Unit.Sessions;

public sealed class SessionsSeedDataFixture : IDisposable
{
    public DataContext DataContext { get; private set; }

    public SessionsSeedDataFixture()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase("MovieListDatabase")
            .Options;

        DataContext = new DataContext(options);

        Seed();

    }

    private void Seed()
    {
        // DataContext.Sessions.Add(...);
    }

    public void Dispose()
    {
        DataContext.Database.EnsureDeleted();
        DataContext.Dispose();
    }
}

public class CreateHandlerTests
{
    private readonly Mock<DataContext> _mockContext;
    private readonly Mock<IValidator<Application.Sessions.Create.Command>> _mockValidator;
    private readonly Application.Sessions.Create.Handler _handler;

    public CreateHandlerTests()
    {
        _mockContext = new Mock<DataContext>(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options);
        _mockValidator = new Mock<IValidator<Application.Sessions.Create.Command>>();
        _handler = new Application.Sessions.Create.Handler(_mockContext.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var session = new Session { Id = 1, TagId = 1, Description = "Test Session" };
        var command = new Application.Sessions.Create.Command { Session = session };

        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockContext.Setup(c => c.Tags.FirstAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tag { Id = 1 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ShouldReturnFailure()
    {
        // Arrange
        var session = new Session { Id = 1, TagId = 1, Description = "Test Session" };
        var command = new Application.Sessions.Create.Command { Session = session };

        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[] { new FluentValidation.Results.ValidationFailure("Session", "Invalid") }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_EmptyDescription_ShouldSetDefaultDescription()
    {
        // Arrange
        var session = new Session { Id = 1, TagId = 1, Description = "" };
        var command = new Application.Sessions.Create.Command { Session = session };

        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockContext.Setup(c => c.Tags.FirstAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tag, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tag { Id = 1 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("-", session.Description);
    }
}