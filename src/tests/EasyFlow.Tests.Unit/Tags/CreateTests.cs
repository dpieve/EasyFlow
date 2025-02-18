using EasyFlow.Application.Tags;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EasyFlow.Tests.Unit.Tags;

public sealed class CreateTests : IDisposable
{
    private readonly DataContext _context;
    private readonly Mock<IValidator<Create.Command>> _validatorMock;
    private readonly Create.Handler _handler;

    public CreateTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);

        _validatorMock = new Mock<IValidator<Create.Command>>();

        _handler = new Create.Handler(_context, _validatorMock.Object);
    }

    [Fact]
    public async Task Handler_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var command = new Create.Command { Tag = new() { Name = string.Empty } };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Tag", "Tag name is required") }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TagsErrors.BadRequest, result.Error);
    }

    [Fact]
    public async Task Handler_ShouldReturnFailure_WhenMaxTagsExceeded()
    {
        // Arrange
        for (int i = 0; i <= Tag.MaxNumTags; i++)
        {
            _context.Tags.Add(new Tag { Name = $"Tag{i}" });
        }

        await _context.SaveChangesAsync();

        var command = new Create.Command { Tag = new() { Name = "Tests" } };
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TagsErrors.CanNotMoreThanMax, result.Error);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted(); // Clean up after each test
        _context.Dispose();
    }
}