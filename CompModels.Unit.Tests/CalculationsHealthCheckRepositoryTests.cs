using CompModels.DAL.Models;
using CompModels.Repositories.Repositories;
using CompModels.Unit.Tests.Helpers;
using CompModels.Unit.Tests.TestBase;
using FluentAssertions;
using WebStruct.Shared;

namespace CompModels.Unit.Tests;

public class CalculationsHealthCheckRepositoryTests : InMemoryTestBase
{
    private readonly CalculationsHealthCheckRepository _repository;

    public CalculationsHealthCheckRepositoryTests()
    {
        _repository = new CalculationsHealthCheckRepository("TestConnection");
        ReflectionHelper.SetPrivateField(_repository, "_pgContext", _context);
    }

    [Fact]
    public async Task GetFailedCalculationsAsync_WhenFailedRequestsExist_ReturnsDictionary()
    {
        // Arrange
        var failedRequests = new[]
        {
            new BezierCalculationRequest
            {
                X = 1, Y = 1, Z = 1, FibreDiameter = 10, DesiredPorosity = 0.5f,
                UserRequesterId = 1, RequestedAt = DateTime.Now,
                RequestStatusId = (short)RequestStatusesEnum.Error
            },
            new BezierCalculationRequest
            {
                X = 2, Y = 2, Z = 2, FibreDiameter = 20, DesiredPorosity = 0.6f,
                UserRequesterId = 2, RequestedAt = DateTime.Now,
                RequestStatusId = (short)RequestStatusesEnum.Error
            }
        };

        await _context.BezierCalculationRequests.AddRangeAsync(failedRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetFailedCalculationsAsync();

        // Assert
        result.Should().ContainKey("Bezier");
        result["Bezier"].Should().HaveCount(2);
        result["Bezier"].Should().Contain(new[] { failedRequests[0].Id, failedRequests[1].Id });
    }

    [Fact]
    public async Task GetFailedCalculationsAsync_WhenNoFailedRequests_ReturnsEmptyDictionary()
    {
        // Act
        var result = await _repository.GetFailedCalculationsAsync();

        // Assert
        result.Should().BeEmpty();
    }
}