using CompModels.Repositories.Repositories;
using CompModels.Unit.Tests.Helpers;
using CompModels.Unit.Tests.TestBase;
using FluentAssertions;

namespace CompModels.Unit.Tests;

public class RequestStatusesRepositoryTests : InMemoryTestBase
{
    private readonly RequestStatusesRepository _repository;

    public RequestStatusesRepositoryTests()
    {
        _repository = new RequestStatusesRepository("TestConnection");
        ReflectionHelper.SetPrivateField(_repository, "_pgContext", _context);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllStatuses()
    {
        // Act
        var result = await _repository.GetAsync();

        // Assert
        result.Should().HaveCount(4);
        result.Should().ContainKeys(1, 2, 3, 4);
        result[1].Should().Be("New");
        result[2].Should().Be("InProgress");
        result[3].Should().Be("Success");
        result[4].Should().Be("Error");
    }
}