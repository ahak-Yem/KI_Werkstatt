using BookingPlatform.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Models;
using BookingPlatform.EmailManager;

namespace BookingPlatform.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public UserController(AppDbContext context, IWebHostEnvironment env)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                _context = context;
                _env = env;
            }
        }
        public async Task<IActionResult> Index()
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                return View(await _context.Resources.ToListAsync());
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Index(string suche)
        {
            ViewData["Getressourcedetailss"] = suche;
            var empquery = from x in _context.Resources select x;
            if (!string.IsNullOrEmpty(suche))
            {
                empquery = empquery.Where(x => x.Name.Contains(suche));
            }
            return View(await empquery.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                if (id == null || _context.Resources == null)
                {
                    return NotFound();
                }

                var ressource = await _context.Resources
                    .FirstOrDefaultAsync(m => m.ResourceID == id);
                if (ressource == null)
                {
                    return NotFound();
                }
                return View(ressource);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBooking(Booking bookingDetails)
        {
            if (bookingDetails.EndDate < bookingDetails.StartDate)
            {
                ModelState.AddModelError("EndDate", "Das eingegebene Rückgabedatum liegt vor dem Reservierungsdatum");
            }
            if (bookingDetails.EndDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("EndDate", "Das eingegebene Rückgabedatum liegt in der Vergangenheit");
            }
            if (bookingDetails.StartDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("StartDate", "Das eingegebene Reservierungsdatum liegt in der Vergangenheit");
            }
            if (ModelState.IsValid)
            {
                _context.Bookings.Add(bookingDetails);
                _context.SaveChanges();
                string crntUserID = User.Identity.Name;
                EmailsManager eManager = new EmailsManager($"{crntUserID}@htw-berlin.de");
                eManager.SetNewBooking(bookingDetails);
                Resources? crntResource = _context.Resources.Find(bookingDetails.ResourceID);
                eManager.SetRessource(crntResource);
                eManager.CreateAndSendMessage(Mail.bookingconfirmation);
                return RedirectToAction("Index", "User");
            }
            else
                return View(bookingDetails);
        }
        public IActionResult CreateBooking(string? ResourceID)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                if (string.IsNullOrWhiteSpace(ResourceID))
                {
                    return NotFound();
                }
                Resources? resourceFromDB = _context.Resources.Find(int.Parse(ResourceID));
                if (resourceFromDB == null)
                {
                    return NotFound();
                }
                if (resourceFromDB.Quantity == 0)
                {
                    return RedirectToAction("Index", "User");
                }
                Booking newBooking = new Booking();
                newBooking.ResourceID = int.Parse(ResourceID);
                newBooking.BookingCondition = "gebucht";
                newBooking.WarningEmailState = false;
                if (User.Identity.Name != null && User.Identity.IsAuthenticated)
                {
                    newBooking.MatrikelNr = User.Identity.Name;
                }
                return View(newBooking);
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult MeineBuchung()
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                IEnumerable<Booking> BookingsList = _context.Bookings;
                return View(BookingsList);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MeineBuchungVerlängern(int? BookingID)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                if (BookingID == 0 || BookingID == null)
                {
                    return NotFound();
                }
                Booking? bookingFromDB = _context.Bookings.Find(BookingID);
                if (bookingFromDB == null)
                {
                    return NotFound();
                }
                return View(bookingFromDB);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MeineBuchungVerlängern(Booking bookingData)
        {
            Booking? oldBooking = _context.Bookings.Find(bookingData.BookingID);
            if (bookingData.EndDate < bookingData.StartDate)
            {
                ModelState.AddModelError("EndDate", "Das eingegebene Rückgabedatum liegt vor dem Reservierungsdatum");
            }
            if (bookingData.EndDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("EndDate", "Das eingegebene Rückgabedatum liegt in der Vergangenheit");
            }
            if (bookingData.EndDate <= oldBooking.EndDate)
            {
                ModelState.AddModelError("EndDate", "Das eingegebene Rückgabedatum soll nach dem alten Rückgabedatum liegen");
            }
            if (ModelState.IsValid)
            {
                EmailsManager eManager = new EmailsManager($"{bookingData.MatrikelNr}@htw-berlin.de");
                if (oldBooking != null)
                {
                    eManager.SetOldBooking(oldBooking);
                }
                _context.Bookings.Update(bookingData);
                _context.SaveChanges();
                eManager.SetNewBooking(bookingData);
                Resources? crntResource = _context.Resources.Find(bookingData.ResourceID);
                eManager.SetRessource(crntResource);
                eManager.CreateAndSendMessage(Mail.extendconfirmation);
                return RedirectToAction("Index", "User");
            }
            else
                return View(bookingData);
        }

        public IActionResult MeineBuchungstornieren(int? BookingID)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                if (BookingID == 0 || BookingID == null)
                {
                    return NotFound();
                }
                Booking? bookingFromDB = _context.Bookings.Find(BookingID);
                if (bookingFromDB == null)
                {
                    return NotFound();
                }
                return View(bookingFromDB);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MeineBuchungstornieren(Booking boo)
        {
            EmailsManager eManager = new EmailsManager($"{boo.MatrikelNr}@htw-berlin.de");
            if (ModelState.IsValid)
            {
                boo.BookingCondition = "storniert";
                Resources? crntResource = _context.Resources.Find(boo.ResourceID);
                eManager.SetRessource(crntResource);
                eManager.SetOldBooking(boo);
                _context.Bookings.Update(boo);
                eManager.SetRessource(crntResource);
                eManager.SetOldBooking(boo);
                _context.SaveChanges();
                eManager.CreateAndSendMessage(Mail.cancelconfirmation);
                return RedirectToAction("Index", "User");
            }
            else
            {
                return View(boo);
            }
        }
    }
}
