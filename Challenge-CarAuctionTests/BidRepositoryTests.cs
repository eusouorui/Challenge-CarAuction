using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

public class BidRepositoryTests
{
    private readonly DbContextOptions<AuctionDbContext> _options;
    private readonly Random _random;

    public BidRepositoryTests()
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

    #region Tests for IBidRepository methods

    [Fact]
    public async Task FindBidsByAuctionId_Should_Return_Bids_For_Given_AuctionId()
    {
        // Arrange
        var auctionId = GetRandomId();
        var bidOne = new Bid { Id = GetRandomId(), Value = 1000, AuctionId = auctionId };
        var bidTwo = new Bid { Id = GetRandomId(), Value = 2000, AuctionId = auctionId };
        var otherAuctionBid = new Bid { Id = GetRandomId(), Value = 1500, AuctionId = GetRandomId() };

        using (var context = CreateContext())
        {
            context.Bids.AddRange(bidOne, bidTwo, otherAuctionBid);
            await context.SaveChangesAsync();
        }

        // Act
        IEnumerable<Bid> result;
        using (var context = CreateContext())
        {
            var repository = new BidRepository(context);
            result = await repository.FindBidsByAuctionId(auctionId);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, b => b.Id == bidOne.Id && b.Value == bidOne.Value);
        Assert.Contains(result, b => b.Id == bidTwo.Id && b.Value == bidTwo.Value);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Bid()
    {
        // Arrange
        var auctionId = GetRandomId();
        var bid = new Bid { Id = GetRandomId(), Value = 1200, AuctionId = auctionId };

        using (var context = CreateContext())
        {
            var repository = new BidRepository(context);

            // Act
            await repository.AddAsync(bid);
        }

        // Assert
        using (var context = CreateContext())
        {
            var result = await context.Bids.FindAsync(bid.Id);
            Assert.NotNull(result);
            Assert.Equal(bid.Id, result.Id);
            Assert.Equal(bid.Value, result.Value);
            Assert.Equal(bid.AuctionId, result.AuctionId);
        }
    }

    [Fact]
    public async Task InvalidBidsForGivenAuction_Should_Return_False_If_Bid_Is_Valid()
    {
        // Arrange
        var auctionId = GetRandomId();
        var auction = new Auction { Id = auctionId, IsActive = true };
        var bid = new Bid { Id = GetRandomId(), Value = 2000, AuctionId = auctionId };

        using (var context = CreateContext())
        {
            context.Auctions.Add(auction);
            context.Bids.Add(bid);
            await context.SaveChangesAsync();
        }

        // Act
        bool isInvalid;
        using (var context = CreateContext())
        {
            var repository = new BidRepository(context);
            isInvalid = await repository.CheckForInvaldiBidsForGivenAuction(bid);
        }

        // Assert
        Assert.False(isInvalid);
    }

    #endregion
}
