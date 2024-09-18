using ChallengeCarAuction.Models;

namespace Challenge_CarAuction.Data.Repositories
{
    public interface IManufacturerRepository
    {
        Task<IEnumerable<Manufacturer>> FindAllAsync();
        Task<Manufacturer> FindByIdAsync(int id);
        Task AddAsync(Manufacturer manufacturer);
        Task UpdateAsync(Manufacturer manufacturer);
        Task DeleteAsync(int? id);
        Task<bool> ExistsInDb(int id);
        Task<bool> NameExistsInDb(string name);
    }
}
