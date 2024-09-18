using ChallengeCarAuction;
using ChallengeCarAuction.Data.Repository;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuctionTests;

public class ModelRepositoryTests
{
    private readonly DbContextOptions<AuctionDbContext> _options;
    private readonly Random _random;

    public ModelRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<AuctionDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _random = new Random();
    }

    private AuctionDbContext CreateContext()
    {
        return new AuctionDbContext(_options);
    }

    private int GetRandomId()
    {
        return _random.Next(1, 10000);
    }

    [Fact]
    public async Task FindAllAsync_Should_Return_All_Models()
    {
        // Arrange
        var firstId = GetRandomId();
        var secondId = GetRandomId();
        var ModelOne = new Model { Id = firstId, Name = $"Model {firstId}" };
        var ModelTwo = new Model { Id = secondId, Name = $"Model {secondId}" };

        using (var context = CreateContext())
        {
            context.Models.AddRange(ModelOne, ModelTwo);
            await context.SaveChangesAsync();
        }

        // Act
        IEnumerable<Model> result;
        using (var context = CreateContext())
        {
            var repository = new ModelRepository(context);
            result = await repository.FindAllAsync();
        }

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, m => m.Name == $"Model {firstId}");
        Assert.Contains(result, m => m.Name == $"Model {secondId}");
    }

    [Fact]
    public async Task FindByIdAsync_Should_Return_Model()
    {
        // Arrange
        var id = GetRandomId();
        var Model = new Model { Id = id, Name = $"Model {id}" };

        using (var context = CreateContext())
        {
            context.Models.Add(Model);
            await context.SaveChangesAsync();
        }

        // Act
        Model result;
        using (var context = CreateContext())
        {
            var repository = new ModelRepository(context);
            result = await repository.FindByIdAsync(id);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"Model {id}", result.Name);
    }


    [Fact]
    public async Task AddAsync_Should_Add_Model()
    {
        // Arrange
        var id = GetRandomId();
        var Model = new Model { Id = id, Name = $"Model {id}" };

        using (var context = CreateContext())
        {
            var repository = new ModelRepository(context);

            // Act
            await repository.AddAsync(Model);
        }

        // Assert
        using (var context = CreateContext())
        {
            var result = await context.Models.FindAsync(id);
            Assert.NotNull(result);
            Assert.Equal($"Model {id}", result.Name);
        }
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Model()
    {
        // Arrange
        var id = GetRandomId();
        var Model = new Model { Id = id, Name = "Old Name" };
        using (var context = CreateContext())
        {
            context.Models.Add(Model);
            await context.SaveChangesAsync();
        }

        Model.Name = "New Name";
        using (var context = CreateContext())
        {
            var repository = new ModelRepository(context);
            await repository.UpdateAsync(Model);
        }

        // Assert
        using (var context = CreateContext())
        {
            var updatedModel = await context.Models.FindAsync(id);
            Assert.NotNull(updatedModel);
            Assert.Equal("New Name", updatedModel.Name);
        }
    }
    [Fact]
    public async Task DeleteAsync_Should_Remove_Model()
    {
        // Arrange
        var id = GetRandomId();
        var Model = new Model { Id = id, Name = $"Model {id}" };
        using (var context = CreateContext())
        {
            context.Models.Add(Model);
            await context.SaveChangesAsync();
        }

        using (var context = CreateContext())
        {
            var repository = new ModelRepository(context);
            await repository.DeleteAsync(id);
        }

        // Assert
        using (var context = CreateContext())
        {
            var deletedModel = await context.Models.FindAsync(id);
            Assert.Null(deletedModel);
        }
    }


    [Fact]
    public async Task ExistsInDb_Should_Return_True_If_Model_Exists()
    {
        // Arrange
        var id = GetRandomId();
        var Model = new Model { Id = id, Name = $"Model {id}" };
        using (var context = CreateContext())
        {
            context.Models.Add(Model);
            await context.SaveChangesAsync();
        }

        // Act
        bool exists;
        using (var context = CreateContext())
        {
            var repository = new ModelRepository(context);
            exists = await repository.ExistsAsync(id);
        }

        // Assert
        Assert.True(exists);
    }


    [Fact]
    public async Task ExistsInDb_Should_Return_False_If_Model_Does_Not_Exist()
    {
        // Act
        bool exists;
        using (var context = CreateContext())
        {
            var repository = new ModelRepository(context);
            exists = await repository.ExistsAsync(GetRandomId());
        }

        // Assert
        Assert.False(exists);
    }
}