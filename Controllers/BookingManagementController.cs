using Microsoft.AspNetCore.Mvc;
using BookingPlatform.Data;
using BookingPlatform.Models;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.EmailManager;
using System.Collections.Specialized;

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
                foreach (Booking bookingData in BookingsList)
                {
                    if (bookingData.EndDate < DateTime.Now.Date && bookingData.BookingCondition != "zurückgegeben")
                    {
                        bookingData.BookingCondition = "abgelaufen";
                        _db.Bookings.Update(bookingData);
                    }
                }
                _db.SaveChanges();
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
            if (LoginController.GetUserType() == "admin")
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
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>
        public IActionResult Bestätigen(int? BookingID)
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
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Bestätigen(Booking boo)
        {
            if (LoginController.GetUserType() == "admin")
            {
                Booking? oldBooking = _db.Bookings.Find(boo.BookingID);
                if (ModelState.IsValid)
                {
                    if (boo.BookingCondition == "reserviert")
                    {
                        Booking newboo = new Booking();
                        var resource = _db.Resources.Find(boo.ResourceID);
                        boo.BookingCondition = newboo.BookingCondition = "gebucht";
                        resource.Quantity -= 1;
                        _db.Resources.Update(resource);
                        _db.Bookings.Update(boo);
                        _db.SaveChanges();
                        string crntUserID = User.Identity.Name;
                        EmailsManager eManager = new EmailsManager($"{crntUserID}@htw-berlin.de");
                        eManager.SetNewBooking(boo);
                        Resources? crntResource = _db.Resources.Find(boo.ResourceID);
                        eManager.SetRessource(crntResource);
                        eManager.CreateAndSendMessage(Mail.bookingconfirmation);
                        return RedirectToAction("Index", "BookingManagement");
                    }
                    if (boo.BookingCondition == "Stornierung beantragt")
                    {
                        EmailsManager eManager = new EmailsManager($"{boo.MatrikelNr}@htw-berlin.de");
                        if (ModelState.IsValid)
                        {
                            Booking newboo = new Booking();
                            boo.BookingCondition = newboo.BookingCondition = "storniert";
                            Resources ? crntResource = _db.Resources.Find(boo.ResourceID);
                            eManager.SetRessource(crntResource);
                            eManager.SetOldBooking(boo);
                            crntResource.Quantity++;
                            _db.Bookings.Update(boo);
                            _db.Resources.Update(crntResource);
                            eManager.SetRessource(crntResource);
                            eManager.SetOldBooking(boo);
                            _db.SaveChanges();
                            eManager.CreateAndSendMessage(Mail.cancelconfirmation);
                            return RedirectToAction("Index", "BookingManagement");
                        }
                        else
                            return View(boo);
                    }
                    if (boo.BookingCondition == "Verlängerung beantragt")
                    {
                        if (ModelState.IsValid)
                        {
                            EmailsManager eManager = new EmailsManager($"{boo.MatrikelNr}@htw-berlin.de");
                            if (oldBooking != null)
                            {
                                eManager.SetOldBooking(oldBooking);
                            }
                            boo.BookingCondition = "verlängert";
                            _db.Bookings.Update(boo);
                            _db.SaveChanges();
                            eManager.SetNewBooking(boo);
                            Resources? crntResource = _db.Resources.Find(boo.ResourceID);
                            eManager.SetRessource(crntResource);
                            eManager.CreateAndSendMessage(Mail.extendconfirmation);
                            return RedirectToAction("Index", "BookingManagement");
                        }
                        else
                            return View(boo);
                    }
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>
        public IActionResult Absagen(int? BookingID)
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
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Absagen(Booking boo)
        {
            if (LoginController.GetUserType() == "admin")
            {
                Booking? oldBooking = _db.Bookings.Find(boo.BookingID);
                if (ModelState.IsValid)
                {
                    if (boo.BookingCondition == "reserviert")
                    {
                        Booking newboo = new Booking();
                        boo.BookingCondition = newboo.BookingCondition = "Reservierung abgesagt";
                        _db.Bookings.Update(boo);
                        _db.SaveChanges();
                        return RedirectToAction("Index", "BookingManagement");
                    }
                    if (boo.BookingCondition == "Stornierung beantragt")
                    {
                        Booking newboo = new Booking();
                        boo.BookingCondition = newboo.BookingCondition = "Stornierung abgesagt";
                        _db.Bookings.Update(boo);
                        _db.SaveChanges();
                        return RedirectToAction("Index", "BookingManagement");
                    }
                    if (boo.BookingCondition == "Verlängerung beantragt")
                    {
                        Booking newboo = new Booking();
                        boo.BookingCondition = newboo.BookingCondition = "Verlängerung abgesagt";
                        _db.Bookings.Update(boo);
                        _db.SaveChanges();
                        return RedirectToAction("Index", "BookingManagement");
                    }
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>

        public IActionResult Abgeholt(int? BookingID)
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
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Abgeholt(Booking boo)
        {
            if (LoginController.GetUserType() == "admin")
            {
                if (ModelState.IsValid)
                {
                    if (boo.BookingCondition == "gebucht")
                    {
                        Booking newboo = new Booking();
                        boo.BookingCondition = newboo.BookingCondition = "abgeholt";
                        _db.Bookings.Update(boo);
                        _db.SaveChanges();
                        return RedirectToAction("Index", "BookingManagement");
                    }
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>
        public IActionResult Zurückgegeben(int? BookingID)
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
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zurückgegeben(Booking boo)
        {
            if (LoginController.GetUserType() == "admin")
            {
                if (ModelState.IsValid)
                {
                    // Find the resource associated with the booking
                    var resource = _db.Resources.Find(boo.ResourceID);
                    // Increment the resource's quantity
                    resource.Quantity++;
                    // Update the booking's condition
                    boo.BookingCondition = "zurückgegeben";
                    // Update the booking and resource in the database
                    _db.Bookings.Update(boo);
                    _db.Resources.Update(resource);
                    _db.SaveChanges();
                    // Redirect to the index action
                    return RedirectToAction("Index", "BookingManagement");
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}