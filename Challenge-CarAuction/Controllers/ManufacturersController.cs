using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuction.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public ManufacturersController(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        // GET: Manufacturers
        public async Task<IActionResult> Index()
        {
            return View(await _manufacturerRepository.FindAllAsync());
        }

        // GET: Manufacturers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manufacturer = await _manufacturerRepository.FindByIdAsync(id.Value);

            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // GET: Manufacturers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manufacturers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool existsInDb = await _manufacturerRepository.NameExistsInDb(manufacturer.Name);

                    if (existsInDb)
                    {
                        // Entity exists, show a message
                        ViewBag.AlertMessage = $"An entity named {manufacturer.Name} already exists.";
                        return View(manufacturer);
                    }
                    else
                    {
                        await _manufacturerRepository.AddAsync(manufacturer);
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch
                {
                    throw;
                }
            }
            return View(manufacturer);
        }

        // GET: Manufacturers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manufacturer = await _manufacturerRepository.FindByIdAsync(id.Value);

            if (manufacturer == null)
            {
                return NotFound();
            }
            return View(manufacturer);
        }

        // POST: Manufacturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Manufacturer manufacturer)
        {
            if (id != manufacturer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _manufacturerRepository.UpdateAsync(manufacturer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _manufacturerRepository.ExistsInDb(manufacturer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(manufacturer);
        }

        // GET: Manufacturers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manufacturer = await _manufacturerRepository.FindByIdAsync(id.Value);
            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // POST: Manufacturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _manufacturerRepository.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
