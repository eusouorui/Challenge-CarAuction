using ChallengeCarAuction.Models;

namespace Challenge_AuctionAuction.Data.Repositories
{
    public interface IAuctionRepository
    {
        Task<IEnumerable<Auction>> FindAllAsync();
        Task<IEnumerable<Auction>> FindAllWithCarAsync();
        Task<Auction> FindByIdAsync(int id);
        Task<Auction> FindByIdWithCarAsync(int id);
        Task AddAsync(Auction Auction);
        Task UpdateAsync(Auction Auction);
        Task<bool> ExistsInDb(int id);
    }
}
