using ChallengeCarAuction;
using ChallengeCarAuction.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Challenge_CarAuction.Controllers
{
    public class BidsController : Controller
    {
        private readonly AuctionDbContext _context;

        public BidsController(AuctionDbContext context)
        {
            _context = context;
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
                var startingBid = _context.Cars.FirstOrDefault(c=> c.Id == _context.Auctions.FirstOrDefault(a => a.Id == bid.AuctionId).CarId).StartingBid;

                if (_context.Bids.Any(b => b.AuctionId == bid.AuctionId && b.Value > bid.Value))
                {
                    TempData["AlertMessage"] = "Bid must be higher than the current highest bid.";
                }
                else if (bid.Value <= startingBid)
                {
                    TempData["AlertMessage"] = "Bid must be higher than the starting bid of " + startingBid;
                }
                else
                {
                    _context.Add(bid);
                    await _context.SaveChangesAsync();
                }
            }
            return Redirect("/Auctions/Details/" + bid.AuctionId);
        }
    }
}
