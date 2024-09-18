using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeCarAuction.Data
{
    public class CarRepository : ICarRepository
    {
        private readonly AuctionDbContext _context;

        public CarRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Car>> FindAllAsync()
        {
            return await _context.Cars
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> FindAllWithModelAsync()
        {
            return await _context.Cars
                .Include(c => c.Model)
                .ToListAsync();
        }

        public async Task<Car> FindByIdAsync(int id)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Car> FindByIdWithModelAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Model)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int? id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsInDb(int id)
        {
            return await _context.Cars.AnyAsync(c => c.Id == id);
        }
    }
}
