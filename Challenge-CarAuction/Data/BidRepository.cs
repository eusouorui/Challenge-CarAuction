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

        Task<IEnumerable<Bid>> IBidRepository.FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Bid>> FindAllAsync()
        {
            return await _context.Bids
                .Include(b => b.Auction)
                .ToListAsync();
        }

        Task<Bid> IBidRepository.FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Bid> FindByIdAsync(int id)
        {
            return await _context.Bids
                .Include(b => b.Auction)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Bid bid)
        {
            await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bids.AnyAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Bid>> FindBidsByAuctionId(int auctionId)
        {
            return await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .ToListAsync();
        }
    }
}
