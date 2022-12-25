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
            if (LoginController.GetUserType() == "admin")
            {
                _db = db;
            }
        }

        public IActionResult Index()
        {
            if (LoginController.GetUserType() == "admin")
            {
                IEnumerable<Booking> BookingsList = _db.Bookings;
                return View(BookingsList);
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// (GET)
        /// </summary>
        /// <returns></returns
        public IActionResult EditBooking(int? BookingID)
        {

            if (LoginController.GetUserType() == "admin")
            {
                if (BookingID == 0 || BookingID == null)
                {
                    return NotFound();
                }
                Booking? bookingFromDB = _db.Bookings.Find(BookingID);
                if (bookingFromDB == null)
                {
                    return NotFound();
                }
                return View(bookingFromDB);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// (POST)
        /// </summary>
        /// <param name="adminData"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditBooking(Booking bookingData)
        {
            if (ModelState.IsValid)
            {
                _db.Bookings.Update(bookingData);
                _db.SaveChanges();
                return RedirectToAction("Index", "BookingManagement");
            }
            else
                return View(bookingData);
        }
    }
}
