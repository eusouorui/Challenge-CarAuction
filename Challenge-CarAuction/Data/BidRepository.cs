using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuction.Data.Repositories
{
    public class BidRepository : IBidRepository
    {
        private readonly AuctionDbContext _context;

        public BidRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Bid bid)
        {
            await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Bid>> FindBidsByAuctionId(int auctionId)
        {
            return await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .ToListAsync();
        }

        public async Task<bool> CheckForInvaldiBidsForGivenAuction(Bid bid)
        {
            return await _context.Bids.AnyAsync(b => b.AuctionId == bid.AuctionId && b.Value > bid.Value);
        }
    }
}
