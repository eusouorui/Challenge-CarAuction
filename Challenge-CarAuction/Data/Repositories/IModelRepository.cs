using ChallengeCarAuction.Models;

namespace Challenge_CarAuction.Data.Repositories
{
    public interface IModelRepository
    {
        Task<IEnumerable<Model>> FindAllAsync();
        Task<IEnumerable<Model>> FindAllWithManufacturersAsync();
        Task<Model> FindByIdAsync(int id);
        Task<Model> FindByIdWithManufacturerAsync(int id);
        Task AddAsync(Model model);
        Task UpdateAsync(Model model);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}