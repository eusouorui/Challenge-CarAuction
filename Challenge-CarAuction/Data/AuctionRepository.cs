using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace Challenge_AuctionAuction.Data.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AuctionDbContext _context;

        public AuctionRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Auction>> FindAllAsync()
        {
            return await _context.Auctions
                .ToListAsync();
        }

        public async Task<IEnumerable<Auction>> FindAllWithCarAsync()
        {
            return await _context.Auctions
                .Include(a => a.Car)
                .ToListAsync();
        }

        public async Task<Auction> FindByIdAsync(int id)
        {
            return await _context.Auctions
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        
        public async Task<Auction> FindByIdWithCarAsync(int id)
        {
            return await _context.Auctions
                .Include(a => a.Car)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Auction auction)
        {
            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Auction auction)
        {
            _context.Entry(auction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task<bool> ExistsInDb(int id)
        {
            return await _context.Auctions.AnyAsync(a => a.Id == id);
        }
    }
}
