using Microsoft.AspNetCore.Mvc;
using BookingPlatform.Data;
using BookingPlatform.Models;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.EmailManager;

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


        /// <summary>
        /// (GET)
        /// </summary>
        /// <returns></returns
        public IActionResult EditBooking(int? BookingID)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>
        public IActionResult Bestätigen(int? BookingID)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Bestätigen(Booking boo)
        {
            Booking? oldBooking = _db.Bookings.Find(boo.BookingID);
            if (ModelState.IsValid)
            {
                if (boo.BookingCondition == "reserviert")
                {

                  


                    Booking newboo = new Booking();
                    var resource = _db.Resources.Find(boo.ResourceID);
                    boo.BookingCondition=  newboo.BookingCondition = "gebucht";
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
                        Resources? crntResource = _db.Resources.Find(boo.ResourceID);
                        eManager.SetRessource(crntResource);
                        eManager.SetOldBooking(boo);
                        _db.Bookings.Update(boo);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>
        public IActionResult Absagen(int? BookingID)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Absagen(Booking boo)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>

        public IActionResult Abgeholt(int? BookingID)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Abgeholt(Booking boo)
        {
            if(ModelState.IsValid)
            {
                if(boo.BookingCondition == "gebucht")
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BookingID"></param>
        /// <returns></returns>
        public IActionResult Zurückgegeben(int? BookingID)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zurückgegeben(Booking boo)
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

   /// <summary>
   /// 
   /// </summary>
   /// <param name="BookingID"></param>
   /// <returns></returns>
        public IActionResult Isabgelaufen(int? BookingID)
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
            return RedirectToAction("Index", "BookingManagement");

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookingData"></param>
        /// <returns></returns>
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Isabgelaufen(Booking bookingData)
        {
            if (ModelState.IsValid)
            {
               if(bookingData.EndDate.ToString("dd.MM.yyyy").CompareTo(DateTime.Now.ToString("dd.MM.yyyy")) == 1 && bookingData.BookingCondition != "zurückgegeben")
               {
                    bookingData.BookingCondition = "abgelaufen";
                    _db.Bookings.Update(bookingData);
                    _db.SaveChanges();
                   
               }
                return RedirectToAction("Index", "BookingManagement");

            }
            else
                return RedirectToAction("Index", "BookingManagement");

        }





    }
 }
