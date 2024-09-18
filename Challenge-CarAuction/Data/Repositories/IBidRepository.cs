using ChallengeCarAuction.Models;

namespace Challenge_CarAuction.Data.Repositories
{
    public interface IBidRepository
    {
        Task<IEnumerable<Bid>> FindAllAsync();
        Task<Bid> FindByIdAsync(int id);
        Task<IEnumerable<Bid>> FindBidsByAuctionId(int auctionId);
        Task AddAsync(Bid model);
        Task<bool> ExistsAsync(int id);
    }
}