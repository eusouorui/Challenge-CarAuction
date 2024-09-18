using Challenge_AuctionAuction.Data.Repositories;
using Challenge_CarAuction.Data.Repositories;
using ChallengeCarAuction.Models;
using Microsoft.AspNetCore.Mvc;

namespace Challenge_CarAuction.Controllers
{
    public class BidsController : Controller
    {
        private readonly IBidRepository _bidRepository;
        private readonly IAuctionRepository _auctionRepository;
        private readonly ICarRepository _carRepository;

        public BidsController(IBidRepository bidRepository,
                              IAuctionRepository auctionRepository,
                              ICarRepository carRepository)
        {
            _bidRepository = bidRepository;
            _auctionRepository = auctionRepository;
            _carRepository = carRepository;
        }

        // POST: Bids/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value,AuctionId")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                var startingBid = _carRepository.FindByIdAsync(_auctionRepository.FindByIdAsync(bid.AuctionId).Result.CarId).Result.StartingBid;

                if (await _bidRepository.CheckForInvaldiBidsForGivenAuction(bid))
                {
                    TempData["AlertMessage"] = "Bid must be higher than the current highest bid.";
                }
                else if (bid.Value <= startingBid)
                {
                    TempData["AlertMessage"] = "Bid must be higher than the starting bid of " + startingBid;
                }
                else
                {
                    _bidRepository.AddAsync(bid);
                }
            }
            return Redirect("/Auctions/Details/" + bid.AuctionId);
        }
    }
}
