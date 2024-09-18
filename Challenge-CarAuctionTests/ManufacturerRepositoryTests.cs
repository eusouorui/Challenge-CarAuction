using Challenge_CarAuction.Data;
using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuctionTests;

public class ManufacturerRepositoryTests
{
    private readonly DbContextOptions<AuctionDbContext> _options;
    private readonly Random _random;

    public ManufacturerRepositoryTests()
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
    public async Task FindAllAsync_Should_Return_All_Manufacturers()
    {
        // Arrange
        var firstId = GetRandomId();
        var secondId = GetRandomId();
        var manufacturerOne = new Manufacturer { Id = firstId, Name = $"Manufacturer {firstId}" };
        var manufacturerTwo = new Manufacturer { Id = secondId, Name = $"Manufacturer {secondId}" };

        using (var context = CreateContext())
        {
            context.Manufacturers.AddRange(manufacturerOne, manufacturerTwo);
            await context.SaveChangesAsync();
        }

        // Act
        IEnumerable<Manufacturer> result;
        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);
            result = await repository.FindAllAsync();
        }

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, m => m.Name == $"Manufacturer {firstId}");
        Assert.Contains(result, m => m.Name == $"Manufacturer {secondId}");
    }

    [Fact]
    public async Task FindByIdAsync_Should_Return_Manufacturer()
    {
        // Arrange
        var id = GetRandomId();
        var manufacturer = new Manufacturer { Id = id, Name = $"Manufacturer {id}" };

        using (var context = CreateContext())
        {
            context.Manufacturers.Add(manufacturer);
            await context.SaveChangesAsync();
        }

        // Act
        Manufacturer result;
        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);
            result = await repository.FindByIdAsync(id);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"Manufacturer {id}", result.Name);
    }


    [Fact]
    public async Task AddAsync_Should_Add_Manufacturer()
    {
        // Arrange
        var id = GetRandomId();
        var manufacturer = new Manufacturer { Id = id, Name = $"Manufacturer {id}" };

        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);

            // Act
            await repository.AddAsync(manufacturer);
        }

        // Assert
        using (var context = CreateContext())
        {
            var result = await context.Manufacturers.FindAsync(id);
            Assert.NotNull(result);
            Assert.Equal($"Manufacturer {id}", result.Name);
        }
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Manufacturer()
    {
        // Arrange
        var id = GetRandomId();
        var manufacturer = new Manufacturer { Id = id, Name = "Old Name" };
        using (var context = CreateContext())
        {
            context.Manufacturers.Add(manufacturer);
            await context.SaveChangesAsync();
        }

        manufacturer.Name = "New Name";
        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);
            await repository.UpdateAsync(manufacturer);
        }

        // Assert
        using (var context = CreateContext())
        {
            var updatedManufacturer = await context.Manufacturers.FindAsync(id);
            Assert.NotNull(updatedManufacturer);
            Assert.Equal("New Name", updatedManufacturer.Name);
        }
    }
    [Fact]
    public async Task DeleteAsync_Should_Remove_Manufacturer()
    {
        // Arrange
        var id = GetRandomId();
        var manufacturer = new Manufacturer { Id = id, Name = $"Manufacturer {id}" };
        using (var context = CreateContext())
        {
            context.Manufacturers.Add(manufacturer);
            await context.SaveChangesAsync();
        }

        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);
            await repository.DeleteAsync(id);
        }

        // Assert
        using (var context = CreateContext())
        {
            var deletedManufacturer = await context.Manufacturers.FindAsync(id);
            Assert.Null(deletedManufacturer);
        }
    }


    [Fact]
    public async Task ExistsInDb_Should_Return_True_If_Manufacturer_Exists()
    {
        // Arrange
        var id = GetRandomId();
        var manufacturer = new Manufacturer { Id = id, Name = $"Manufacturer {id}" };
        using (var context = CreateContext())
        {
            context.Manufacturers.Add(manufacturer);
            await context.SaveChangesAsync();
        }

        // Act
        bool exists;
        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);
            exists = await repository.ExistsInDb(id);
        }

        // Assert
        Assert.True(exists);
    }


    [Fact]
    public async Task ExistsInDb_Should_Return_False_If_Manufacturer_Does_Not_Exist()
    {
        // Act
        bool exists;
        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);
            exists = await repository.ExistsInDb(GetRandomId()); 
        }

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task NameExistsInDb_Should_Return_False_If_Name_Does_Not_Exist()
    {
        // Act
        bool exists;
        using (var context = CreateContext())
        {
            var repository = new ManufacturerRepository(context);
            exists = await repository.NameExistsInDb("Nonexistent Manufacturer");
        }

        // Assert
        Assert.False(exists);
    }

}