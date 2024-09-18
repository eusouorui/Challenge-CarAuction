using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeCarAuction.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Range(1970, 2024)]
        public int ModelYear { get; set; }

        public VehicleType VehicleType { get; set; }

        public bool? HasActiveAuction { get; set; }

        [Range(100, 100000)]
        public decimal StartingBid { get; set; }

        [Range(1, 7)]
        public int? NumberOfDoors { get; set; }

        [Range(1, 55)]
        public int? NumberOfSeats { get; set; }

        [Range(100, 10000)]
        public decimal? LoadCapacity { get; set; }

        // External propertiies / keys
        public int ModelId { get; set; }

        public Model? Model { get; set; }

        [NotMapped]
        public Manufacturer? Manufacturer { get; set; }
    }
}
