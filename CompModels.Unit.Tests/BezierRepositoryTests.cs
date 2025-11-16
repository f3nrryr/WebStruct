using CompModels.DAL.Models;
using CompModels.Repositories.DTOs.In.Bezier;
using CompModels.Repositories.Repositories;
using CompModels.Unit.Tests.Helpers;
using CompModels.Unit.Tests.TestBase;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebStruct.Shared;

namespace CompModels.Unit.Tests;

public class BezierRepositoryTests : InMemoryTestBase
{
    private readonly BezierRepository _repository;

    public BezierRepositoryTests()
    {
        _repository = new BezierRepository("TestConnection");
        ReflectionHelper.SetPrivateField(_repository, "_pgContext", _context);
    }

    [Fact]
    public async Task GetRequestIdToHandleChronologicAsync_WhenNewRequestsExist_ReturnsOldestId()
    {
        // Arrange
        var requests = new[]
        {
            new BezierCalculationRequest
            {
                X = 1, Y = 1, Z = 1, FibreDiameter = 10, DesiredPorosity = 0.5f,
                UserRequesterId = 1, RequestedAt = DateTime.Now.AddMinutes(-10),
                RequestStatusId = (short)RequestStatusesEnum.New
            },
            new BezierCalculationRequest
            {
                X = 2, Y = 2, Z = 2, FibreDiameter = 20, DesiredPorosity = 0.6f,
                UserRequesterId = 2, RequestedAt = DateTime.Now.AddMinutes(-5),
                RequestStatusId = (short)RequestStatusesEnum.New
            }
        };

        await _context.BezierCalculationRequests.AddRangeAsync(requests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRequestIdToHandleChronologicAsync();

        // Assert
        result.Should().Be(requests[0].Id);
    }

    [Fact]
    public async Task GetRequestIdToHandleChronologicAsync_WhenNoNewRequests_ReturnsZero()
    {
        // Act
        var result = await _repository.GetRequestIdToHandleChronologicAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetRequestInputDataForCalculateAsyncById_WhenRequestExists_ReturnsData()
    {
        // Arrange
        var request = new BezierCalculationRequest
        {
            X = 10, Y = 20, Z = 30, FibreDiameter = 15, DesiredPorosity = 0.7f,
            UserRequesterId = 1, RequestedAt = DateTime.Now,
            RequestStatusId = (short)RequestStatusesEnum.New
        };

        await _context.BezierCalculationRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRequestInputDataForCalculateAsyncById(request.Id);

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(request.X);
        result.Y.Should().Be(request.Y);
        result.Z.Should().Be(request.Z);
        result.FibreDiameter.Should().Be((int)request.FibreDiameter);
        result.DesiredPorosity.Should().Be(request.DesiredPorosity);
    }

    [Fact]
    public async Task GetRequestInputDataForCalculateAsyncById_WhenRequestNotExists_ReturnsNull()
    {
        // Act
        var result = await _repository.GetRequestInputDataForCalculateAsyncById(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void AddCalculationRequest_WithValidInput_ReturnsNewId()
    {
        // Arrange
        var input = new BezierInputParamsValues
        {
            X = 5, Y = 5, Z = 5, FibreDiameter = 10,
            DesiredPorosity = 0.5f, UserRequesterId = 1
        };

        // Act
        var result = _repository.AddCalculationRequest(input);

        // Assert
        result.Should().BeGreaterThan(0);
        _context.BezierCalculationRequests.Should().Contain(x => x.Id == result);
    }

    [Fact]
    public async Task GetCalculationRequestStatusIdAsync_WhenRequestExists_ReturnsStatusId()
    {
        // Arrange
        var request = new BezierCalculationRequest
        {
            X = 1, Y = 1, Z = 1, FibreDiameter = 10, DesiredPorosity = 0.5f,
            UserRequesterId = 1, RequestedAt = DateTime.Now,
            RequestStatusId = (short)RequestStatusesEnum.New
        };

        await _context.BezierCalculationRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCalculationRequestStatusIdAsync((int)request.Id, 1);

        // Assert
        result.Should().Be((short)RequestStatusesEnum.New);
    }

    [Fact]
    public async Task GetCalculationRequestStatusIdAsync_WhenUserNotMatch_ReturnsZero()
    {
        // Arrange
        var request = new BezierCalculationRequest
        {
            X = 1, Y = 1, Z = 1, FibreDiameter = 10, DesiredPorosity = 0.5f,
            UserRequesterId = 1, RequestedAt = DateTime.Now,
            RequestStatusId = (short)RequestStatusesEnum.New
        };

        await _context.BezierCalculationRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCalculationRequestStatusIdAsync((int)request.Id, 999);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetFinishedCalculationOutputAsync_WhenFinishedRequestExists_ReturnsResponse()
    {
        // Arrange
        var request = new BezierCalculationRequest
        {
            X = 1, Y = 1, Z = 1, FibreDiameter = 10, DesiredPorosity = 0.5f,
            UserRequesterId = 1, RequestedAt = DateTime.Now,
            RequestStatusId = (short)RequestStatusesEnum.Success,
            CalcResultPorosity = 0.45f
        };

        await _context.BezierCalculationRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetFinishedCalculationOutputAsync((int)request.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result.InputParamsValues.X.Should().Be(request.X);
        result.OutputParamsValues.CalcResultPorosity.Should().Be(0.45f);
    }

    [Fact]
    public async Task FinishCalculationRequestAsync_WithValidData_UpdatesRequestAndCreatesFile()
    {
        // Arrange
        var request = new BezierCalculationRequest
        {
            X = 1, Y = 1, Z = 1, FibreDiameter = 10, DesiredPorosity = 0.5f,
            UserRequesterId = 1, RequestedAt = DateTime.Now,
            RequestStatusId = (short)RequestStatusesEnum.New
        };

        await _context.BezierCalculationRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        var resultFile = new PhysicalFile
        {
            FileNameWithoutDotAndExtension = "result",
            Extension = "txt",
            FileContent = new byte[] { 1, 2, 3, 4, 5 }
        };

        // Act
        await _repository.FinishCalculationRequestAsync(request.Id, 0.48f, resultFile);

        // Assert
        var updatedRequest = await _context.BezierCalculationRequests.FindAsync(request.Id);
        updatedRequest.RequestStatusId.Should().Be((short)RequestStatusesEnum.Success);
        updatedRequest.CalcResultPorosity.Should().Be(0.48f);

        var file = await _context.BezierResultsPhysicalFiles.FirstAsync();
        file.FileNameWithoutDotAndExtension.Should().Be("result");
        file.Extension.Should().Be("txt");

        var fileBind = await _context.BezierRequestFiles.FirstAsync();
        fileBind.ExperimentId.Should().Be(request.Id);
    }

    [Fact]
    public async Task FinishCalculationRequestAsync_WhenRequestNotFound_ThrowsException()
    {
        // Arrange
        var resultFile = new PhysicalFile
        {
            FileNameWithoutDotAndExtension = "result",
            Extension = "txt",
            FileContent = new byte[] { 1, 2, 3 }
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _repository.FinishCalculationRequestAsync(999, 0.5f, resultFile));
    }

    [Fact]
    public async Task SetRequestFailedAsync_WhenRequestExists_UpdatesStatus()
    {
        // Arrange
        var request = new BezierCalculationRequest
        {
            X = 1, Y = 1, Z = 1, FibreDiameter = 10, DesiredPorosity = 0.5f,
            UserRequesterId = 1, RequestedAt = DateTime.Now,
            RequestStatusId = (short)RequestStatusesEnum.New
        };

        await _context.BezierCalculationRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        // Act
        await _repository.SetRequestFailedAsync(request.Id);

        // Assert
        var updatedRequest = await _context.BezierCalculationRequests.FindAsync(request.Id);
        updatedRequest.RequestStatusId.Should().Be((short)RequestStatusesEnum.Error);
    }
}