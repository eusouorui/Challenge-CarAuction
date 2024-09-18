using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuction.Controllers
{
    public class ModelsController : Controller
    {
        private readonly IModelRepository _modelRepository;
        private readonly IManufacturerRepository _manufacturerRepository;

        public ModelsController(IModelRepository modelRepository, IManufacturerRepository manufacturerRepository)
        {
            _modelRepository = modelRepository;
            _manufacturerRepository = manufacturerRepository;
        }

        // GET: Models
        public async Task<IActionResult> Index()
        {
            return View(await _modelRepository.FindAllWithManufacturersAsync());
        }

        // GET: Models/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _modelRepository.FindByIdWithManufacturerAsync(id.Value).Result;

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Models/Create
        public IActionResult Create()
        {
            ViewData["Manufacturers"] = GetSelectListOfManufacturers().Result;
            return View();
        }

        // POST: Models/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ManufacturerId")] Model model)
        {
            if (ModelState.IsValid)
            {
                await _modelRepository.AddAsync(model);
               
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManufacturerId"] = new SelectList(await _modelRepository.FindAllAsync(), "Id", "Id", model.ManufacturerId);
            return View(model);
        }

        // GET: Models/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _modelRepository.FindByIdAsync(id.Value);
            if (model == null)
            {
                return NotFound();
            }

            ViewData["Manufacturers"] = GetSelectListOfManufacturers().Result;
            return View(model);
        }

        // POST: Models/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ManufacturerId")] Model model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _modelRepository.UpdateAsync(model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _modelRepository.ExistsAsync(model.Id))
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
            ViewData["Manufacturers"] = GetSelectListOfManufacturers().Result;
            return View(model);
        }

        // GET: Models/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _modelRepository.FindByIdWithManufacturerAsync(id.Value);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Models/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            await _modelRepository.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetSelectListOfManufacturers()
        {
            var manufacturers = new List<SelectListItem>();
            var fetchedManufacturers = await _manufacturerRepository.FindAllAsync();
            foreach (var item in fetchedManufacturers)
            {
                manufacturers.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            
            return manufacturers;
        }
    }
}
