using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuction.Controllers
{
    public class AuctionsController : Controller
    {
        private readonly AuctionDbContext _context;

        public AuctionsController(AuctionDbContext context)
        {
            _context = context;
        }

        // GET: Auctions
        public async Task<IActionResult> Index()
        {
            var auctionDbContext = _context.Auctions.Include(a => a.Car);
            var cars = _context.Cars.ToList();
            var manufacturers = _context.Manufacturers.ToList();
            var models = _context.Models.ToList();
            var carInfoList = new List<string>();

            // todo rd make this a method maybe
            foreach (var item in auctionDbContext)
            {
                item.IsActiveText = item.IsActive ? "Yes" : "No";

                item.Car = cars.Where(c => c.Id == item.CarId)?.FirstOrDefault();
                item.Car.Model = models.Where(m => m.Id == item.Car.ModelId)?.FirstOrDefault();
                var manufacturerName = manufacturers.Where(m => m.Id == item.Car.Model.ManufacturerId)?.FirstOrDefault()?.Name;

                if (!item.Car.Model.Name.Contains(manufacturerName))
                {
                    item.Car.Model.Name = manufacturerName + " " + item.Car.Model.Name;
                }

                carInfoList.Add(GetCarInfoPropertyName(item.Car) + ": " + GetCarInfo(item.Car));
            }

            ViewData["CarInfo"] = carInfoList;

            return View(await auctionDbContext.ToListAsync());
        }

        // GET: Auctions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions
                .Include(a => a.Car)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            auction.AuctionBids = _context.Bids.Where(b => b.AuctionId == auction.Id).ToList();

            auction.Car = _context.Cars.FirstOrDefault(c => c.Id == auction.CarId) ?? new Car();
            var model = _context.Models.Where(c => c.Id == auction.Car.ModelId).FirstOrDefault() ?? new Model();
            var manufacturer = _context.Manufacturers.Where(m => m.Id == model.ManufacturerId).FirstOrDefault() ?? new Manufacturer();

            auction.Car.Model.Name = $"{manufacturer.Name} {model.Name}";

            var AlertMessage = TempData["AlertMessage"] as string;

            ViewData["AlertMessage"] = AlertMessage;
            ViewData["CarInfoName"] = GetCarInfoPropertyName(auction.Car);
            ViewData["CarInfo"] = GetCarInfo(auction.Car);

            return View(auction);
        }

        // GET: Auctions/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = GetSelectListOfModels();
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsActive,CarId")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                auction.IsActive = true;
                _context.Add(auction);
                var car = _context.Cars.Where(c => c.Id.Equals(auction.CarId)).FirstOrDefault();
                car.HasActiveAuction = true;
                _context.Update(car);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(auction);
        }

        // GET: Auctions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }

            var car = _context.Cars.Where(c => c.Id.Equals(auction.CarId)).FirstOrDefault();
            car.Model = _context.Models.Where(m => m.Id == car.ModelId)?.FirstOrDefault();
            car.Model.Manufacturer = _context.Manufacturers.Where(m => m.Id == car.Model.ManufacturerId)?.FirstOrDefault();

            ViewData["CarId"] = new List<SelectListItem> { new SelectListItem { Text = car.Model.Manufacturer.Name + " " + car.Model.Name, Value = car.Id.ToString() } };

            ViewData["IsActive"] = CheckIfCarHasActiveAcution(auction) && !auction.IsActive;
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsActive,CarId")] Auction auction)
        {
            if (id != auction.Id)
            {
                return NotFound();
            }

            var car = _context.Cars.Where(c => c.Id.Equals(auction.CarId)).FirstOrDefault();

            if (car != null) 
            {
                car.Model = _context.Models.Where(m => m.Id == car.ModelId)?.FirstOrDefault();
                car.Model.Manufacturer = _context.Manufacturers.Where(m => m.Id == car.Model.ManufacturerId)?.FirstOrDefault();
                ViewData["CarId"] = new List<SelectListItem> { new SelectListItem { Text = car.Model.Manufacturer.Name + " " + car.Model.Name, Value = car.Id.ToString() } };
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auction);
                    car.HasActiveAuction = auction.IsActive;
                    _context.Update(car);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionExists(auction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Auctions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auctions
                .Include(a => a.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            var model = _context.Models.Where(c => c.Id == auction.Car.ModelId).FirstOrDefault();
            var manufacturer = _context.Manufacturers.Where(m => m.Id == model.ManufacturerId).FirstOrDefault();

            auction.Car.Model.Name = $"{manufacturer.Name} {model.Name}";

            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction != null)
            {
                _context.Auctions.Remove(auction);
            }

            if (auction.IsActive)
            {
                var car = _context.Cars.FirstOrDefault(c => c.Id == auction.CarId);
                if (car.HasActiveAuction == true)
                {
                    car.HasActiveAuction = false;
                    _context.Update(car);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionExists(int id)
        {
            return _context.Auctions.Any(e => e.Id == id);
        }

        private List<SelectListItem> GetSelectListOfModels()
        {
            var cars = _context.Cars.ToList();
            var manufacturers = _context.Manufacturers.ToList();
            var models = _context.Models.ToList();

            var selectListView = new List<SelectListItem>();

            foreach (var car in cars)
            {
                if (!car.HasActiveAuction == true)
                {
                    var model = models.Where(m => m.Id == car.ModelId).FirstOrDefault();
                    var manufacturer = manufacturers.Where(m => m.Id == model.ManufacturerId).FirstOrDefault();

                    selectListView.Add(
                        new SelectListItem
                        {
                            Value = car.Id.ToString(),
                            Text = manufacturer?.Name + " " + model?.Name
                        }
                    );
                }
            }

            return selectListView;
        }

        private bool CheckIfCarHasActiveAcution(Auction auction)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == auction.CarId);

            return car.HasActiveAuction == true;
        }

        private string GetCarInfo(Car? car)
        {
            try
            {
                return car.VehicleType switch
                {
                    VehicleType.Sedan or VehicleType.HatchBack => car.NumberOfDoors.ToString(),
                    VehicleType.SUV => car.NumberOfSeats.ToString(),
                    VehicleType.Truck => car.LoadCapacity.ToString(),
                    _ => string.Empty,
                };
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetCarInfoPropertyName(Car? car)
        {
            try
            {
                return car.VehicleType switch
                {
                    VehicleType.Sedan or VehicleType.HatchBack => "Number of Doors",
                    VehicleType.SUV => "Number of seats",
                    VehicleType.Truck => "Load capacity",
                    _ => string.Empty,
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
