using CompModels.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CompModels.Unit.Tests.TestBase;

public abstract class InMemoryTestBase : IDisposable
{
    protected readonly GenStructContext _context;

    protected InMemoryTestBase()
    {
        var options = new DbContextOptionsBuilder<GenStructContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GenStructContext(options);
        InitializeDatabase();
    }

    protected virtual void InitializeDatabase()
    {
        SeedStatuses();
        SeedComputationalModels();
        _context.SaveChanges();
    }

    private void SeedStatuses()
    {
        if (!_context.RequestsStatusesHandbooks.Any())
        {
            _context.RequestsStatusesHandbooks.AddRange(
                new RequestsStatusesHandbook { Id = 1, Name = "New" },
                new RequestsStatusesHandbook { Id = 2, Name = "InProgress" },
                new RequestsStatusesHandbook { Id = 3, Name = "Success" },
                new RequestsStatusesHandbook { Id = 4, Name = "Error" }
            );
        }
    }

    private void SeedComputationalModels()
    {
        if (!_context.ComputationalModels.Any())
        {
            _context.ComputationalModels.AddRange(
                new ComputationalModel
                {
                    Id = 1,
                    Name = "Bezier Model",
                    Description = "Bezier curve computational model",
                    InputParamsJsonExample = "{\"x\": 1, \"y\": 1, \"z\": 1}",
                    OutputParamsJsonExample = "{\"porosity\": 0.5}",
                    IsCellularAutomaton = false,
                    IsPorousModel = true,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAt = DateTime.Now
                }
            );
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}