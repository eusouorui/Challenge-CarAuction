using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeCarAuction.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        public int ModelYear { get; set; }

        public VehicleType VehicleType { get; set; }

        public bool? HasActiveAuction { get; set; }

        public decimal StartingBid { get; set; }

        public int? NumberOfDoors { get; set; }

        public int? NumberOfSeats { get; set; }

        public decimal? LoadCapacity { get; set; }

        // External propertiies / keys
        public int ModelId { get; set; }

        public Model? Model { get; set; }

        [NotMapped]
        public Manufacturer? Manufacturer { get; set; }
    }
}
