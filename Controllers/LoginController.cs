using BookingPlatform.Data;
using BookingPlatform.LoginManager;
using BookingPlatform.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingPlatform.Controllers
{
    enum userType { nothing=0,admin=1,user=2};
    
    public class LoginController : Controller
    {
        public bool isaccountvalid = false;
        static userType loggedUserType =userType.nothing;
        public static string GetUserType()
        {
            return loggedUserType.ToString();
        }
        private readonly AppDbContext _db;

        public LoginController(AppDbContext db)
        {
            _db = db;

        }
        public async Task<IActionResult> Logout()
        {
            isaccountvalid= false;
            loggedUserType = userType.nothing;
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home"); 
        }

        public async Task<IActionResult> Index(string MatrNr, string Passwort)
        {

            LdapAuthorization ldap = new LdapAuthorization("Login", "login-dc-01.login.htw-berlin.de");
            isaccountvalid = ldap.ValidateByBind(MatrNr,Passwort);

            IEnumerable<Admin> admins = _db.Admins;
            foreach (Admin admin in admins)
            {
                if (isaccountvalid == true && MatrNr == admin.AdminID)
                {
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, MatrNr)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, "Login");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    loggedUserType = userType.admin;
                    return RedirectToAction("Index", "Ressources");
                }

            }
            if (isaccountvalid == false)
            {
                ModelState.AddModelError("Fehler", "Prüfen Sie nochmal Ihre Login-Daten und schalten Sie Ihre HTW-Vpn an!.");
            }
            if (isaccountvalid == true && ModelState.IsValid)
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, MatrNr)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new
                ClaimsPrincipal(claimsIdentity));
                loggedUserType = userType.user;
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
