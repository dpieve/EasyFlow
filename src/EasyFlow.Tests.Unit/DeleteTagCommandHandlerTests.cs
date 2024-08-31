using EasyFlow.Application.Tags;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using Moq;

namespace EasyFlow.Tests.Unit;

public class DeleteTagCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenNotDeleteSelectedTag()
    {
        // Arrange
        var tag = new Tag { Id = 1, Name = "Tag1" };

        var tagsRepositoryMock = new Mock<ITagsRepository>();
        tagsRepositoryMock.Setup(repo => repo.DeleteAsync(tag)).ReturnsAsync(true);

        var generalSettingsRepositoryMock = new Mock<IGeneralSettingsRepository>();
        generalSettingsRepositoryMock.Setup(repo => repo.GetAsync()).ReturnsAsync(new List<GeneralSettings>()
        {
            new GeneralSettings() { SelectedTagId = 2 }
        });

        var command = new DeleteTagCommand { Tag = tag };
        var handler = new DeleteTagCommandHandler(tagsRepositoryMock.Object, generalSettingsRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenDeleteSelectedTag()
    {
        // Arrange
        var tag = new Tag { Id = 1, Name = "Tag1" };
        var tagsRepositoryMock = new Mock<ITagsRepository>();
        tagsRepositoryMock.Setup(repo => repo.DeleteAsync(tag)).ReturnsAsync(true);

        var generalSettingsRepositoryMock = new Mock<IGeneralSettingsRepository>();
        generalSettingsRepositoryMock.Setup(repo => repo.GetAsync()).ReturnsAsync(new List<GeneralSettings>
        {
            new GeneralSettings { SelectedTagId = tag.Id }
        });

        var command = new DeleteTagCommand { Tag = tag };
        var handler = new DeleteTagCommandHandler(tagsRepositoryMock.Object, generalSettingsRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TagsErrors.CanNotDeleteSelectedTag, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenDeleteFail()
    {
        // Arrange
        var tag = new Tag { Id = 1, Name = "Tag1" };
        var tagsRepositoryMock = new Mock<ITagsRepository>();
        tagsRepositoryMock.Setup(repo => repo.DeleteAsync(tag)).ReturnsAsync(false);

        var generalSettingsRepositoryMock = new Mock<IGeneralSettingsRepository>();
        generalSettingsRepositoryMock.Setup(repo => repo.GetAsync()).ReturnsAsync(new List<GeneralSettings>()
        {
            new GeneralSettings() { SelectedTagId = 2 }
        });

        var command = new DeleteTagCommand { Tag = tag };
        var handler = new DeleteTagCommandHandler(tagsRepositoryMock.Object, generalSettingsRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TagsErrors.DeleteFail, result.Error);
    }
}