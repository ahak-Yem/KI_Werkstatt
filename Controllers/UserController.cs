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
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                ViewData["Getressourcedetailss"] = suche;
                var empquery = from x in _context.Resources select x;
                if (!string.IsNullOrEmpty(suche))
                {
                    empquery = empquery.Where(x => x.Name.Contains(suche));
                }
                return View(await empquery.AsNoTracking().ToListAsync());
            }
            return RedirectToAction("Index", "Home");
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
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
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
            return RedirectToAction("Index", "Home");
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
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
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
            return RedirectToAction("Index", "Home");
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
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
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
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult filterByDate(DateTime sDate, DateTime eDate)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                if (DateTime.Compare(sDate, eDate) == -1 && sDate > DateTime.Now && eDate > DateTime.Now)
                {
                    ViewBag.entryValid = true;
                    //Get all ressources and bookings
                    List<Resources> allResources = _context.Resources.ToList<Resources>();
                    List<Booking> allBooking = _context.Bookings.ToList<Booking>();

                    List<Resources> availableResources = new List<Resources>();
                    Dictionary<int, DateTime> specResourceAvaDate = new Dictionary<int, DateTime>();

                    foreach (var resource in allResources)
                    {
                        if (resource.Quantity == 0)
                        {
                            //move through all the bookings
                            foreach (var booking in allBooking)
                            {
                                //take all the bookings with the unavailable resources and save them to a list
                                if (booking.ResourceID == resource.ResourceID)
                                {
                                    //Check if the unavailable resources will be available in the given date and save all available ones
                                    if (DateTime.Compare(booking.EndDate, sDate) == -1 && booking.BookingCondition != "zurückgegeben")
                                    {
                                        if (!availableResources.Contains(resource))
                                        {
                                            availableResources.Add(resource);
                                            specResourceAvaDate.TryAdd(resource.ResourceID, booking.EndDate);
                                            ViewData[resource.ResourceID.ToString()] = booking.EndDate.ToString("dd.MM.yyyy");
                                        }
                                        else if (DateTime.Compare(booking.EndDate, specResourceAvaDate[booking.ResourceID]) == -1)
                                        {
                                            specResourceAvaDate[booking.ResourceID] = booking.EndDate;
                                            ViewData[resource.ResourceID.ToString()] = booking.EndDate.ToString("dd.MM.yyyy");
                                        }
                                    }
                                }
                            }
                        }
                        else if (resource.Quantity > 0)
                        {
                            availableResources.Add(resource);
                        }
                    }
                    return View(availableResources);
                }
                else
                {
                    ViewBag.entryValid = false;
                    return View(_context.Resources.ToList<Resources>());
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AvailabilityCalender(DateTime YAM, int? resourceId, string? warning, string? entryValid, string? availability, string? availableFrom, string? sDate, string? eDate)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                int monat = YAM.Month;
                int jahr = YAM.Year;
                if (monat != 0 && jahr != 0 && YAM != new DateTime())
                {
                    CalenderManager manager = new CalenderManager(jahr, monat, 1);
                    Dictionary<int, string> passDays = manager.SetDaysOfCurrentMonth();
                    ViewBag.Calender = passDays;
                    ViewBag.ChoosenYearAndMonth = YAM;
                    ViewBag.resourceID = resourceId;
                }
                if (warning != null)
                {
                    ViewBag.warning = warning;
                }
                if (entryValid != null)
                {
                    ViewBag.entryValid = entryValid;
                }
                if (sDate != null && eDate != null)
                    {
                        ViewBag.sDate = sDate;
                        ViewBag.eDate = eDate;
                    }
                if (availability != null)
                {
                    ViewBag.availability = availability;
                }
                if (availableFrom != null)
                {
                    ViewBag.availableFrom = availableFrom;
                }

                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult PeriodPicker(IFormCollection formCollection, DateTime YAM, int? resourceID)
        {
            if (LoginController.GetUserType() == "admin" || LoginController.GetUserType() == "user")
            {
                string? EntryValid = null, Warning = null, Availability = null, AvailableFrom = null;
                if (YAM == new DateTime())
                {
                    EntryValid = "false";
                    Warning = "Bitte wählen Sie ein Jahr und Monat";
                    return RedirectToAction("AvailabilityCalender", "User", new { resourceId = resourceID, warning = Warning, entryValid = EntryValid });
                }
                else if (resourceID == 0)
                {
                    EntryValid = "false";
                    Warning = "Bitte gehen Sie zur Hardwareseite zurück und wiederholen Sie das Prozess !";
                    return RedirectToAction("AvailabilityCalender", "User", new { YAM = YAM, warning = Warning, entryValid = EntryValid });
                }
                else if (formCollection != null)
                {
                    CalenderManager manager = new CalenderManager(YAM.Year, YAM.Month, 0);
                    DateTime[] startAndEnd = manager.GetChoosenDays(formCollection);
                    DateTime sDate = startAndEnd[0];
                    DateTime eDate = startAndEnd[1];
                    if (eDate.Year == 0001)
                    {
                        EntryValid = "false";
                        Warning = "Bitte wählen Sie zwei Felder aus !";
                        return RedirectToAction("AvailabilityCalender", "User", new { YAM = YAM, resourceId = resourceID, warning = Warning, entryValid = EntryValid });
                    }
                    Resources? resource = _context.Resources.Find(resourceID);
                    Booking firstAvailable = new Booking();
                    Booking possibleAvailable = new Booking();
                    if (resource.Quantity == 0)
                    {
                        var bookings = from allbookings in _context.Bookings.Where(allBookings => allBookings.ResourceID == resourceID) select allbookings;
                        foreach (var booking in bookings)
                        {
                            if (booking.ResourceID == resource.ResourceID)
                            {
                                if (DateTime.Compare(booking.EndDate, sDate) == -1 && booking.BookingCondition != "zurückgegeben")
                                {
                                    if (firstAvailable.BookingID == 0 && firstAvailable.ResourceID == 0 || firstAvailable.EndDate > booking.EndDate)
                                    {
                                        firstAvailable = booking;
                                    }
                                }
                                else if (booking.BookingCondition != "zurückgegeben")
                                {
                                    if (possibleAvailable.BookingID == 0 && possibleAvailable.ResourceID == 0 || possibleAvailable.EndDate > booking.EndDate)
                                    {
                                        possibleAvailable = booking;
                                    }
                                }
                            }
                        }
                        if (firstAvailable.BookingID != 0 && firstAvailable.ResourceID!=0)
                        {
                            Availability = "bald";
                            AvailableFrom = firstAvailable.EndDate.ToString("dd.MM.yyyy");
                            return RedirectToAction("AvailabilityCalender", "User", new { YAM = YAM, resourceId = resourceID, availability = Availability, availableFrom = AvailableFrom, sDate = sDate.ToString("dd.MM.yyyy"), eDate = eDate.ToString("dd.MM.yyyy") });
                        }
                        else
                        {
                            Availability = "am ausgewählten Zeitraum nicht verfügbar";
                            AvailableFrom = possibleAvailable.EndDate.ToString("dd.MM.yyyy");
                            return RedirectToAction("AvailabilityCalender", "User", new { YAM = YAM, resourceId = resourceID, availability = Availability, availableFrom = AvailableFrom, sDate = sDate.ToString("dd.MM.yyyy"), eDate = eDate.ToString("dd.MM.yyyy") });

                        }
                    }
                    else if (resource.Quantity > 0)
                    {
                        //write the html for this
                        AvailableFrom = DateTime.Now.ToString("dd.MM.yyyy");
                        Availability = "verfügbar";
                        return RedirectToAction("AvailabilityCalender", "User", new { YAM = YAM, resourceId = resourceID, availability = Availability, availableFrom = AvailableFrom, sDate = sDate.ToString("dd.MM.yyyy"), eDate = eDate.ToString("dd.MM.yyyy") });
                    }
                }
                Warning = "Error";
                EntryValid = "false";
                return RedirectToAction("AvailabilityCalender", "User", new { YAM = YAM, resourceId = resourceID, warning = Warning, entryValid = EntryValid });
            }
            return RedirectToAction("Index", "Home");

        }
    }
}