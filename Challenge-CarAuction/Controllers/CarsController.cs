using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuction.Controllers
{
    public class CarsController : Controller
    {
        private readonly AuctionDbContext _context;

        public CarsController(AuctionDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var auctionDbContext = _context.Cars.Include(c => c.Model);
            var manufacturers = _context.Manufacturers.ToList();

            foreach(var item in auctionDbContext)
            {
                item.Manufacturer = manufacturers.Where(m => m.Id == item.Model.ManufacturerId).FirstOrDefault();
            }

            return View(await auctionDbContext.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            ViewData["VehicleTypes"] = GetVehicleTypes();
            ViewData["Models"] = GetSelectListOfModels();
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModelYear,VehicleType,StartingBid,NumberOfDoors,NumberOfSeats,LoadCapacity,ModelId")] Car car)
        {
            ViewData["VehicleTypes"] = GetVehicleTypes();
            ViewData["Models"] = GetSelectListOfModels();

            if (ModelState.IsValid)
            {
                var errorMessage = ValidateValuesByVehicleType(car);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.AlertMessage = errorMessage;
                    return View(car);
                }

                car.HasActiveAuction = false;
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["VehicleTypes"] = GetVehicleTypes();
            ViewData["Models"] = GetSelectListOfModels();
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModelYear,VehicleType,HasActiveAuction,StartingBid,NumberOfDoors,NumberOfSeats,LoadCapacity,ModelId")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            ViewData["VehicleTypes"] = GetVehicleTypes();
            ViewData["Models"] = GetSelectListOfModels();

            if (ModelState.IsValid)
            {
                var errorMessage = ValidateValuesByVehicleType(car);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.AlertMessage = errorMessage;
                    return View(car);
                }

                try
                {
                    car.HasActiveAuction = car.HasActiveAuction ?? false;
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
           
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region Private Methods

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        private List<SelectListItem> GetSelectListOfModels()
        {
            var models = new List<SelectListItem>();
            var manufacturers = _context.Manufacturers.ToList();
            foreach (var item in _context.Models.OrderBy(m => m.Manufacturer.Name).ThenBy(m => m.Name))
            {
                models.Add(new SelectListItem { Text = manufacturers?.Where(m => m.Id == item.ManufacturerId).FirstOrDefault()?.Name + " " + item.Name, Value = item.Id.ToString() });
            }

            return models;
        }

        private List<SelectListItem> GetVehicleTypes()
        {
            var vehicleTypes = new List<SelectListItem>();

            foreach (VehicleType item in Enum.GetValues(typeof(VehicleType)))
            {
                vehicleTypes.Add(new SelectListItem { Text = item.ToString(), Value = (int)item + "" });
            }

            return vehicleTypes;
        }

        private static string ValidateValuesByVehicleType(Car car)
        {
            switch (car.VehicleType)
            {
                case VehicleType.Sedan:
                case VehicleType.HatchBack:
                    if (car.NumberOfDoors == null || car.NumberOfDoors <= 0)
                    {
                        return $"{VehicleType.Sedan}s and {VehicleType.HatchBack.ToString()}s must have at least 1 door.";
                    }
                    break;
                case VehicleType.SUV:
                    if (car.NumberOfSeats == null || car.NumberOfSeats <= 0)
                    {
                        return $"{VehicleType.SUV}s must have the number of doors specified.";
                    }
                    break;
                case VehicleType.Truck:
                    if (car.LoadCapacity == null || car.LoadCapacity <= 0)
                    {
                        return $"{VehicleType.Truck}s must have the load capacity specified.";
                    }
                    break;
            }

            return string.Empty;
        }

        #endregion
    }
}
