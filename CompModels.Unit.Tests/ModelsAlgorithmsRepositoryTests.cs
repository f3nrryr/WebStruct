using CompModels.Repositories.DTOs.In.ModelsAlgorhitms;
using CompModels.Repositories.Repositories;
using CompModels.Unit.Tests.Helpers;
using CompModels.Unit.Tests.TestBase;
using FluentAssertions;
using WebStruct.Shared;

namespace CompModels.Unit.Tests;

public class ModelsAlgorithmsRepositoryTests : InMemoryTestBase
{
    private readonly ModelsAlgorithmsRepository _repository;

    public ModelsAlgorithmsRepositoryTests()
    {
        _repository = new ModelsAlgorithmsRepository("TestConnection");
        ReflectionHelper.SetPrivateField(_repository, "_pgContext", _context);
    }

    [Fact]
    public async Task GetModelNameByIdAsync_WhenModelExists_ReturnsName()
    {
        // Act
        var result = await _repository.GetModelNameByIdAsync(1);

        // Assert
        result.Should().Be("Bezier Model");
    }

    [Fact]
    public async Task GetModelNameByIdAsync_WhenModelNotExists_ReturnsNull()
    {
        // Act
        var result = await _repository.GetModelNameByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetModelsAlgorithmsHandbookAsync_ReturnsAllModels()
    {
        // Act
        var result = await _repository.GetModelsAlgorithmsHandbookAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Bezier Model");
        result[0].IsPorousModel.Should().BeTrue();
    }

    [Fact]
    public void CreateModelAlgorithm_WithValidData_ReturnsNewId()
    {
        // Arrange
        var createDto = new CreateModelAlgorhitm
        {
            Name = "New Model",
            Description = "New model description",
            InputParamsJsonExample = "{}",
            OutputParamsJsonExample = "{}",
            IsCellularAutomaton = true,
            IsPorousModel = false,
            CreatedBy = Guid.NewGuid()
        };

        // Act
        var result = _repository.CreateModelAlgorithm(createDto);

        // Assert
        result.Should().BeGreaterThan(0);
        _context.ComputationalModels.Should().Contain(x => x.Id == result && x.Name == "New Model");
    }

    [Fact]
    public async Task DeleteModelAlgorithmAsync_WhenModelExists_DeletesModel()
    {
        // Arrange
        var modelId = 1;

        // Act
        await _repository.DeleteModelAlgorithmAsync(modelId);

        // Assert
        var model = await _context.ComputationalModels.FindAsync(modelId);
        model.Should().BeNull();
    }

    [Fact]
    public async Task DeleteModelAlgorithmAsync_WhenModelNotExists_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<UsefulException>(() =>
            _repository.DeleteModelAlgorithmAsync(999));
    }
}