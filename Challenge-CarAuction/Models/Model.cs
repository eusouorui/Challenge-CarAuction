using System.ComponentModel.DataAnnotations;

namespace ChallengeCarAuction.Models
{
    public class Model
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        // external values

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public ICollection<Car> Cars { get; set; }
    }
}