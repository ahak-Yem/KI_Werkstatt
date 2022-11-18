using Microsoft.AspNetCore.Mvc;
using BookingPlatform.Data;
using BookingPlatform.Models;
namespace BookingPlatform.Controllers
{
    public class BookingManagementController : Controller
    {
        private readonly AppDbContext _db;

        public BookingManagementController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Booking> BookingsList = _db.Bookings;
            return View(BookingsList);
        }
    }
}
