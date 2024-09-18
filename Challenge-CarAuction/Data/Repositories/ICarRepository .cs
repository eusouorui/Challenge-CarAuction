using ChallengeCarAuction.Models;

namespace Challenge_CarAuction.Data.Repositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> FindAllAsync();
        Task<IEnumerable<Car>> FindAllWithModelAsync();
        Task<Car> FindByIdAsync(int id);
        Task<Car> FindByIdWithModelAsync(int id);
        Task AddAsync(Car Car);
        Task UpdateAsync(Car Car);
        Task DeleteAsync(int? id);
        Task<bool> ExistsInDb(int id);
    }
}
