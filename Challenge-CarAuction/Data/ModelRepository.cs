using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeCarAuction.Data.Repository
{
    public class ModelRepository : IModelRepository
    {
        private readonly AuctionDbContext _context;

        public ModelRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Model>> FindAllAsync()
        {
            return await _context.Models.ToListAsync();
        }

        public async Task<Model> FindByIdAsync(int id)
        {
            return await _context.Models.FindAsync(id);
        }

        public async Task AddAsync(Model model)
        {
            _context.Models.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Model model)
        {
            _context.Models.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var model = await _context.Models.FindAsync(id);
            if (model != null)
            {
                _context.Models.Remove(model);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Models.AnyAsync(m => m.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Models.AnyAsync(m => m.Name == name);
        }

        public async Task<IEnumerable<Model>> FindAllWithManufacturersAsync()
        {
            return await _context.Models.Include(m => m.Manufacturer).ToListAsync();
        }

        public async Task<Model> FindByIdWithManufacturerAsync(int id)
        {
            return await _context.Models.Include(m => m.Manufacturer).FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
