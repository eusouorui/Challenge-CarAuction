using ChallengeCarAuction.Models;

namespace Challenge_CarAuction.Models.ViewModels
{
    public class BidsViewModel
    {
        public Bid Bid { get; set; }
        public IEnumerable<Bid> Bids { get; set; }

        public decimal StartingBid { get; set; }
    }
}
