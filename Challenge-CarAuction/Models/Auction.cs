using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeCarAuction.Models
{
    public class Auction
    {
        [Key]
        public int Id { get; set; }

        public bool IsActive { get; set; }
        
        
        public Car Car { get; set; }

        public List<Bid> AuctionBids { get; set; }
    }
}
