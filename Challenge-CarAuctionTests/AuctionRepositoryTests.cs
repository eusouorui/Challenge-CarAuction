using Challenge_AuctionAuction.Data.Repositories;
using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;


namespace Challenge_CarAuctionTests
{
    public class AuctionRepositoryTests
    {
        private readonly DbContextOptions<AuctionDbContext> _options;
        private readonly Random _random;

        public AuctionRepositoryTests()
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
        public async Task FindAllAsync_Should_Return_All_Auctions()
        {
            // Arrange
            var firstAuctionId = GetRandomId();
            var secondAuctionId = GetRandomId();
            var carOneId = GetRandomId();
            var carTwoId = GetRandomId();

            var carOne = new Car { Id = carOneId, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };
            var carTwo = new Car { Id = carTwoId, ModelYear = 2021, VehicleType = VehicleType.Sedan, StartingBid = 15000, ModelId = GetRandomId() };

            var auctionOne = new Auction { Id = firstAuctionId, CarId = carOneId, IsActive = true };
            var auctionTwo = new Auction { Id = secondAuctionId, CarId = carTwoId, IsActive = false };

            using (var context = CreateContext())
            {
                context.Cars.AddRange(carOne, carTwo);
                context.Auctions.AddRange(auctionOne, auctionTwo);
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<Auction> result;
            using (var context = CreateContext())
            {
                var repository = new AuctionRepository(context);
                result = await repository.FindAllAsync();
            }

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, a => a.Id == firstAuctionId);
            Assert.Contains(result, a => a.Id == secondAuctionId);
        }


        [Fact]
        public async Task FindByIdAsync_Should_Return_Auction()
        {
            // Arrange
            var auctionId = GetRandomId();
            var carId = GetRandomId();

            var car = new Car { Id = carId, ModelYear = 2020, VehicleType = VehicleType.SUV, StartingBid = 5000, ModelId = GetRandomId() };

            var auction = new Auction { Id = auctionId, CarId = carId, IsActive = true };

            using (var context = CreateContext())
            {
                context.Cars.Add(car);
                context.Auctions.Add(auction);
                await context.SaveChangesAsync();
            }

            // Act
            Auction result;
            using (var context = CreateContext())
            {
                var repository = new AuctionRepository(context);
                result = await repository.FindByIdAsync(auctionId);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(auctionId, result.Id);
            Assert.Equal(carId, result.CarId);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Auction()
        {
            // Arrange
            var auctionId = GetRandomId();
            var carId = GetRandomId();
            var auction = new Auction { Id = auctionId, CarId = carId, IsActive = true };

            using (var context = CreateContext())
            {
                var repository = new AuctionRepository(context);

                // Act
                await repository.AddAsync(auction);
            }

            // Assert
            using (var context = CreateContext())
            {
                var result = await context.Auctions.FindAsync(auctionId);
                Assert.NotNull(result);
                Assert.Equal(auctionId, result.Id);
                Assert.Equal(carId, result.CarId);
                Assert.True(result.IsActive);
            }
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Auction()
        {
            // Arrange
            var auctionId = GetRandomId();
            var carId = GetRandomId();
            var auction = new Auction { Id = auctionId, CarId = carId, IsActive = true };

            using (var context = CreateContext())
            {
                context.Auctions.Add(auction);
                await context.SaveChangesAsync();
            }

            // Act
            auction.IsActive = false;
            using (var context = CreateContext())
            {
                var repository = new AuctionRepository(context);
                await repository.UpdateAsync(auction);
            }

            // Assert
            using (var context = CreateContext())
            {
                var updatedAuction = await context.Auctions.FindAsync(auctionId);
                Assert.NotNull(updatedAuction);
                Assert.False(updatedAuction.IsActive);
            }
        }

        [Fact]
        public async Task ExistsInDb_Should_Return_True_If_Auction_Exists()
        {
            // Arrange
            var auctionId = GetRandomId();
            var carId = GetRandomId();
            var auction = new Auction { Id = auctionId, CarId = carId, IsActive = true };

            using (var context = CreateContext())
            {
                context.Auctions.Add(auction);
                await context.SaveChangesAsync();
            }

            // Act
            bool exists;
            using (var context = CreateContext())
            {
                var repository = new AuctionRepository(context);
                exists = await repository.ExistsInDb(auctionId);
            }

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsInDb_Should_Return_False_If_Auction_Does_Not_Exist()
        {
            // Act
            bool exists;
            using (var context = CreateContext())
            {
                var repository = new AuctionRepository(context);
                exists = await repository.ExistsInDb(GetRandomId());  
            }

            // Assert
            Assert.False(exists);
        }
    }
}

