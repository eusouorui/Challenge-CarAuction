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
        public virtual Manufacturer? Manufacturer { get; set; }
    }
}