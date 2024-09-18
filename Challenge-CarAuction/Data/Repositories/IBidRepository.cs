using ChallengeCarAuction.Models;

namespace Challenge_CarAuction.Data.Repositories
{
    public interface IBidRepository
    {
        Task<IEnumerable<Bid>> FindBidsByAuctionId(int auctionId);
        Task AddAsync(Bid model);
        Task<bool> CheckForInvaldiBidsForGivenAuction(Bid bid);
    }
}