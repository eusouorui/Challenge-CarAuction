using System.ComponentModel.DataAnnotations;

namespace ChallengeCarAuction.Models
{
    public class Bid
    {
        [Key]
        public int Id { get; set; }

        public decimal Value { get; set; }

        // External values
        public int AuctionId { get; set; }
        
        public Auction? Auction { get; set; }
    }
}