using Challenge_AuctionAuction.Data.Repositories;
using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Challenge_CarAuction.Controllers
{
    public class AuctionsController : Controller
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly ICarRepository _carRepository;
        private readonly IModelRepository _modelRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IBidRepository _bidRepository;

        public AuctionsController(IAuctionRepository auctionRepository,
                                  ICarRepository carRepository,
                                  IModelRepository modelRepository,
                                  IManufacturerRepository manufacturerRepository,
                                  IBidRepository bidRepository)
        {
            _auctionRepository = auctionRepository;
            _carRepository = carRepository;
            _modelRepository = modelRepository;
            _manufacturerRepository = manufacturerRepository;
            _bidRepository = bidRepository;
        }

        // GET: Auctions
        public async Task<IActionResult> Index()
        {
            var auctionDbContext = await _auctionRepository.FindAllWithCarAsync();
            var cars = await _carRepository.FindAllAsync();
            var manufacturers = await _manufacturerRepository.FindAllAsync();
            var models = await _modelRepository.FindAllAsync();
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

            return View(auctionDbContext);
        }

        // GET: Auctions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _auctionRepository.FindByIdWithCarAsync(id.Value);

            if (auction == null)
            {
                return NotFound();
            }

            auction.AuctionBids = _bidRepository.FindBidsByAuctionId(auction.Id).Result.ToList();

            auction.Car = await _carRepository.FindByIdAsync(auction.CarId);
            auction.Car.Model = await _modelRepository.FindByIdAsync(auction.Car.ModelId);
            auction.Car.Model.Manufacturer = await _manufacturerRepository.FindByIdAsync(auction.Car.Model.ManufacturerId);

            auction.Car.Model.Name = $"{auction.Car.Model.Manufacturer.Name} {auction.Car.Model.Name}";

            var AlertMessage = TempData["AlertMessage"] as string;

            ViewData["AlertMessage"] = AlertMessage;
            ViewData["CarInfoName"] = GetCarInfoPropertyName(auction.Car);
            ViewData["CarInfo"] = GetCarInfo(auction.Car);

            return View(auction);
        }

        // GET: Auctions/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = GetSelectListOfModels().Result;
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

                await _auctionRepository.AddAsync(auction);

                var car = await _carRepository.FindByIdAsync(auction.CarId);
                car.HasActiveAuction = true;

                await _carRepository.UpdateAsync(car);

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

            var auction = await _auctionRepository.FindByIdAsync(id.Value);
            if (auction == null)
            {
                return NotFound();
            }

            var car = await _carRepository.FindByIdAsync(auction.CarId);
            car.Model = await _modelRepository.FindByIdAsync(car.Id);
            car.Model.Manufacturer = await _manufacturerRepository.FindByIdAsync(car.Model.ManufacturerId);


            ViewData["CarId"] = new List<SelectListItem> { new() { Text = car.Model.Manufacturer.Name + " " + car.Model.Name, Value = car.Id.ToString() } };

            ViewData["IsActive"] = CheckIfCarHasActiveAcution(auction).Result && !auction.IsActive;
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

            var car = await _carRepository.FindByIdAsync(auction.CarId);

            if (car != null) 
            {
                car.Model = await _modelRepository.FindByIdAsync(car.ModelId);
                car.Model.Manufacturer = await _manufacturerRepository.FindByIdAsync(car.Model.ManufacturerId);

                ViewData["CarId"] = new List<SelectListItem> { new SelectListItem { Text = car.Model.Manufacturer.Name + " " + car.Model.Name, Value = car.Id.ToString() } };
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _auctionRepository.UpdateAsync(auction);
                    car.HasActiveAuction = auction.IsActive;
                    await _carRepository.UpdateAsync(car);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _auctionRepository.ExistsInDb(auction.Id))
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

        private async Task<List<SelectListItem>> GetSelectListOfModels()
        {
            var cars = await _carRepository.FindAllAsync();
            var manufacturers = await _manufacturerRepository.FindAllAsync();
            var models = await _modelRepository.FindAllAsync();

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

        private async Task<bool> CheckIfCarHasActiveAcution(Auction auction)
        {
            var car = await _carRepository.FindByIdAsync(auction.Id);

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
