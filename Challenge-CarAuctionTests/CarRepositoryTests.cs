using ChallengeCarAuction;
using ChallengeCarAuction.Data;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuctionTests
{
    public class CarRepositoryTests
    {
        private readonly DbContextOptions<AuctionDbContext> _options;
        private readonly Random _random;

        public CarRepositoryTests()
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
        public async Task FindAllAsync_Should_Return_All_Cars()
        {
            // Arrange
            var firstId = GetRandomId();
            var secondId = GetRandomId();
            var carOne = new Car { Id = firstId, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };
            var carTwo = new Car { Id = secondId, ModelYear = 2021, VehicleType = VehicleType.Sedan, StartingBid = 15000, ModelId = GetRandomId() };

            using (var context = CreateContext())
            {
                context.Cars.AddRange(carOne, carTwo);
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<Car> result;
            using (var context = CreateContext())
            {
                var repository = new CarRepository(context);
                result = await repository.FindAllAsync();
            }

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.Id == firstId);
            Assert.Contains(result, c => c.Id == secondId);
        }


        [Fact]
        public async Task FindByIdAsync_Should_Return_Car()
        {
            // Arrange
            var id = GetRandomId();
            var car = new Car { Id = id, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };

            using (var context = CreateContext())
            {
                context.Cars.Add(car);
                await context.SaveChangesAsync();
            }

            // Act
            Car result;
            using (var context = CreateContext())
            {
                var repository = new CarRepository(context);
                result = await repository.FindByIdAsync(id);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Car()
        {
            // Arrange
            var id = GetRandomId();
            var car = new Car { Id = id, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };

            using (var context = CreateContext())
            {
                var repository = new CarRepository(context);

                // Act
                await repository.AddAsync(car);
            }

            // Assert
            using (var context = CreateContext())
            {
                var result = await context.Cars.FindAsync(id);
                Assert.NotNull(result);
                Assert.Equal(id, result.Id);
            }
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Car()
        {
            // Arrange
            var id = GetRandomId();
            var car = new Car { Id = id, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };
            using (var context = CreateContext())
            {
                context.Cars.Add(car);
                await context.SaveChangesAsync();
            }

            car.StartingBid = 7000;
            using (var context = CreateContext())
            {
                var repository = new CarRepository(context);
                await repository.UpdateAsync(car);
            }

            // Assert
            using (var context = CreateContext())
            {
                var updatedCar = await context.Cars.FindAsync(id);
                Assert.NotNull(updatedCar);
                Assert.Equal(7000, updatedCar.StartingBid);
            }
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Car()
        {
            // Arrange
            var id = GetRandomId();
            var car = new Car { Id = id, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };
            using (var context = CreateContext())
            {
                context.Cars.Add(car);
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                var repository = new CarRepository(context);
                await repository.DeleteAsync(id);
            }

            // Assert
            using (var context = CreateContext())
            {
                var deletedCar = await context.Cars.FindAsync(id);
                Assert.Null(deletedCar);
            }
        }

        [Fact]
        public async Task ExistsInDb_Should_Return_True_If_Car_Exists()
        {
            // Arrange
            var id = GetRandomId();
            var car = new Car { Id = id, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };
            using (var context = CreateContext())
            {
                context.Cars.Add(car);
                await context.SaveChangesAsync();
            }

            // Act
            bool exists;
            using (var context = CreateContext())
            {
                var repository = new CarRepository(context);
                exists = await repository.ExistsInDb(id);
            }

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsInDb_Should_Return_False_If_Car_Does_Not_Exist()
        {
            // Act
            bool exists;
            using (var context = CreateContext())
            {
                var repository = new CarRepository(context);
                exists = await repository.ExistsInDb(GetRandomId());
            }

            // Assert
            Assert.False(exists);
        }
    }
}
