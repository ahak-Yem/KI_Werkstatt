using BookingPlatform.LoginManager;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.Controllers
{
    public class LoginController : Controller
    {
        public bool isaccountvalid = false;


        public IActionResult Index(string MatrNr, string Passwort)
        {
            LdapAuthorization ldap = new LdapAuthorization("Login", "login-dc-01.login.htw-berlin.de");
            isaccountvalid = ldap.ValidateByBind($"{MatrNr}", $"{Passwort}");

            if (isaccountvalid == false)
            {
                ModelState.AddModelError("Fehler", "Prüfen Sie nochmal Ihre Login-Daten und schalten Sie Ihre HTW-Vpn an!.");
            }
            if (isaccountvalid == true && ModelState.IsValid)
            {
                isaccountvalid = true;
                return RedirectToAction("Index", "Ressources");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
