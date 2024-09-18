using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeCarAuction.Data
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly AuctionDbContext _context;

        public ManufacturerRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Manufacturer>> FindAllAsync()
        {
            return await _context.Manufacturers.ToListAsync();
        }

        public async Task<Manufacturer> FindByIdAsync(int id)
        {
            return await _context.Manufacturers.FindAsync(id);
        }

        public async Task AddAsync(Manufacturer manufacturer)
        {
            await _context.Manufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Manufacturer manufacturer)
        {
            _context.Entry(manufacturer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int? id)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer != null)
            {
                _context.Manufacturers.Remove(manufacturer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> NameExistsInDb(string name)
        {
            return await _context.Manufacturers.AnyAsync(m => m.Name.Equals(name));
        }

        public async Task<bool> ExistsInDb(int id)
        {
            return await _context.Manufacturers.AnyAsync(m => m.Id.Equals(id));
        }
    }
}
