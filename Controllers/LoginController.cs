using BookingPlatform.Data;
using BookingPlatform.LoginManager;
using BookingPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.Controllers
{
    public class LoginController : Controller
    {
        public bool isaccountvalid = false;


        private readonly AppDbContext _db;
       
        public LoginController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string MatrNr, string Passwort)
        {
            
            LdapAuthorization ldap = new LdapAuthorization("Login", "login-dc-01.login.htw-berlin.de");
            isaccountvalid = ldap.ValidateByBind($"{MatrNr}", $"{Passwort}");

            IEnumerable<Admin> admins = _db.Admins;
            if (isaccountvalid == true && ModelState.IsValid)
            {
                foreach (Admin admin in admins)
                {
                    if (MatrNr == admin.AdminID)
                    {
                        return RedirectToAction("Index", "Ressources");
                    }
                }
                return RedirectToAction("Index", "User");
            }
            else if (isaccountvalid == false)
            {
                ModelState.AddModelError("Fehler", "Prüfen Sie nochmal Ihre Login-Daten und schalten Sie Ihre HTW-Vpn an!.");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
