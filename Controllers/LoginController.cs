using BookingPlatform.LoginManager;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.Controllers
{
    public class LoginController : Controller
    {
        public bool isaccountvalid = false;
        //A var to be used to save the return value of the method that will check if the data is for a registered admin(Method implemented by Heltonn)
        public bool isAdmin = true;
        public IActionResult Index(string MatrNr, string Passwort)
        {            
            LdapAuthorization ldap = new LdapAuthorization("Login", "login-dc-01.login.htw-berlin.de");
            isaccountvalid = ldap.ValidateByBind(MatrNr,Passwort);
            if (isaccountvalid == false)
            {
                ModelState.AddModelError("Fehler", "Prüfen Sie nochmal Ihre Login-Daten und schalten Sie Ihre HTW-Vpn an!.");
            }
            //Only in case the user that tried to login is an admin.
            else if (isaccountvalid == true && ModelState.IsValid && isAdmin==true)
            {
                //Save the data of the logged in admin using the singleton pattern
                CurrentAdmin crntAdmin=CurrentAdmin.Instance;
                crntAdmin.SetAdminID(MatrNr);
                return RedirectToAction("Index", "Ressources");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
