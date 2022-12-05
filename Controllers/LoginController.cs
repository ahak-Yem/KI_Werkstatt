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
            foreach (Admin admin in admins)
            {
                if (isaccountvalid == true && MatrNr == admin.AdminID)
                {
                    return RedirectToAction("Index", "Ressources");
                }
            }


            if (isaccountvalid == false)
            {
                ModelState.AddModelError("Fehler", "Prüfen Sie nochmal Ihre Login-Daten und schalten Sie Ihre HTW-Vpn an!.");
            }
            if (isaccountvalid == true && ModelState.IsValid)
            {
                isaccountvalid = true;
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
