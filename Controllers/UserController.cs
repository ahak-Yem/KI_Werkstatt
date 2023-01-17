using BookingPlatform.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Models;
using BookingPlatform.EmailManager;
using BookingPlatform.Controllers;

namespace BookingPlatform.Controllers
{
    public class UserController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        
        public UserController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.Resources.ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBooking(Booking bookingDetails)
        {
          
            if (bookingDetails.EndDate<bookingDetails.StartDate)
            {
                ModelState.AddModelError("EndDate", "Das eingegebene Rückgabedatum liegt vor dem Reservierungsdatum");
            }
            if (bookingDetails.EndDate<DateTime.Now.Date)
            {
                ModelState.AddModelError("EndDate", "Das eingegebene Rückgabedatum liegt in der Vergangenheit");
            }
            if (bookingDetails.StartDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("StartDate", "Das eingegebene Reservierungsdatum liegt in der Vergangenheit");
            }
            if (ModelState.IsValid)
            {
                string crntUserID = User.Identity.Name;
                Resources? crntResource = _context.Resources.Find(bookingDetails.ResourceID);
                _context.Bookings.Add(bookingDetails);
                _context.SaveChanges();
                IEnumerable<Admin> admins = _context.Admins;
                foreach (Admin admin in admins)
                {
                    EmailsManager eManager = new EmailsManager($"{admin.AdminID}@htw-berlin.de");

                    eManager.SetNewBooking(bookingDetails);

                    eManager.SetRessource(crntResource);
                    eManager.CreateAndSendMessage(Mail.adminbuchung);

                }
                   
               
              
                return RedirectToAction("Index", "User");
            }
          

            else
                return View(bookingDetails);
        }
        public IActionResult CreateBooking(string? ResourceID)
        {
            
            if (string.IsNullOrWhiteSpace(ResourceID))
            {
                
                return NotFound();
            }
            Resources? resourceFromDB = _context.Resources.Find(int.Parse(ResourceID));
            if(resourceFromDB == null)
            {
                return NotFound();
            }
            if (resourceFromDB.Quantity <1)
            {
                return BadRequest("Nicht mehr verfügbar");
                
            }
            //resourceFromDB.Quantity -= 1;
            //_context.Update(resourceFromDB);
            //_context.SaveChanges();
            Booking newBooking = new Booking();
            newBooking.ResourceID = int.Parse(ResourceID);
            newBooking.BookingCondition = "reserviert";
            newBooking.WarningEmailState = false;
            if (User.Identity.Name != null && User.Identity.IsAuthenticated)
            {
                newBooking.MatrikelNr = User.Identity.Name;
            }
            return View(newBooking);
        }
        public IActionResult MeineBuchung()
        {
            IEnumerable<Booking> BookingsList = _context.Bookings;
            return View(BookingsList);
        }

        public IActionResult MeineBuchungVerlängern(int? BookingID)
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
                string crntUserID = User.Identity.Name;
                Resources? crntResource = _context.Resources.Find(bookingData.ResourceID);
                //EmailsManager eManager = new EmailsManager($"{bookingData.MatrikelNr}@htw-berlin.de");
                //if (oldBooking != null)
                //{
                //    eManager.SetOldBooking(oldBooking);
                //}
                bookingData.BookingCondition = "Verlängerung beantragt";
                _context.Bookings.Update(bookingData);
                _context.SaveChanges();
                //eManager.SetNewBooking(bookingData);
                //Resources? crntResource = _context.Resources.Find(bookingData.ResourceID);
                //eManager.SetRessource(crntResource);
                //eManager.CreateAndSendMessage(Mail.extendconfirmation);

                IEnumerable<Admin> admins = _context.Admins;
                foreach (Admin admin in admins)
                {
                    EmailsManager eManager = new EmailsManager($"{admin.AdminID}@htw-berlin.de");

                    eManager.SetNewBooking(bookingData);

                    eManager.SetRessource(crntResource);
                    eManager.CreateAndSendMessage(Mail.adminverlan);

                }
                return RedirectToAction("Verlängern", "User");
            }
            else
                return View(bookingData);
        }

        public IActionResult MeineBuchungstornieren(int? BookingID)
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
                return View(boo);

        }

        public IActionResult Verlängern(Booking boo)
        {
            //boo.BookingCondition = "Verlängerung beantragt";
            
            //_context.Bookings.Update(boo);
            //_context.SaveChanges();
            return View();
        }

        public IActionResult Stornieren(Booking boo)
        {
            boo.BookingCondition = "Stornierung beantragt";
            _context.Bookings.Update(boo);
            _context.SaveChanges();
            return View();
        }

    

    }
}
