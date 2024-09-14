using System.ComponentModel.DataAnnotations;

namespace ChallengeCarAuction.Models
{
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Model> Models { get; set; }
    }
}