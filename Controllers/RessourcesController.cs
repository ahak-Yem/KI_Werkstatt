using Microsoft.AspNetCore.Mvc;
using BookingPlatform.Data;
using BookingPlatform.Models;
using Microsoft.EntityFrameworkCore;
namespace BookingPlatform.Controllers
{
    public class RessourcesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public RessourcesController(AppDbContext context, IWebHostEnvironment env)
        {
            if (LoginController.GetUserType() == "admin")
            {
                _context = context;
                _env = env;
            }
        }
        public async Task<IActionResult> Index()
        {
            if (LoginController.GetUserType() == "admin")
            {
                return View(await _context.Resources.ToListAsync());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string suche)
        {
            if (LoginController.GetUserType() == "admin")
            {
                ViewData["Getressourcedetails"] = suche;
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

            if (LoginController.GetUserType() == "admin")
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
        // GET: Ressources/Create
        public IActionResult Create(int Id)
        {

            if (LoginController.GetUserType() == "admin")
            {
                if (Id == 0)
                    return View(new Resources());
                else
                    return View(_context.Resources.Find(Id));
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Ressources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Resources ressource)
        {
            if (LoginController.GetUserType() == "admin")
            {
                ModelState.Remove("Buchungen");
                if (ModelState.IsValid)
                {
                    if (ressource.ImageFile != null)
                    {
                        var folder = "images/";
                        folder += Guid.NewGuid().ToString() + "_" + ressource.ImageFile.FileName;
                        ressource.ImageName = folder;
                        var serverFolder = Path.Combine(_env.WebRootPath, folder);

                        await ressource.ImageFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    }

                    _context.Add(ressource);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(ressource);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Ressources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (LoginController.GetUserType() == "admin")
            {
                if (id == null || _context.Resources == null)
                {
                    return NotFound();
                }

                var ressource = await _context.Resources.FindAsync(id);
                if (ressource == null)
                {
                    return NotFound();
                }
                return View(ressource);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Ressources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Resources ressource)
        {
            if (LoginController.GetUserType() == "admin")
            {
                if (id != ressource.ResourceID)
                {
                    return NotFound();
                }
                ModelState.Remove("Buchungen");
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (ressource.ImageFile != null)
                        {
                            var folder = "images/";
                            folder += Guid.NewGuid().ToString() + "_" + ressource.ImageFile.FileName;
                            ressource.ImageName = folder;
                            var serverFolder = Path.Combine(_env.WebRootPath, folder);

                            await ressource.ImageFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                        }

                        _context.Update(ressource);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RessourceExists(ressource.ResourceID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(ressource);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Ressources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (LoginController.GetUserType() == "admin")
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

        // POST: Ressources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (LoginController.GetUserType() == "admin")
            {
                if (_context.Resources == null)
                {
                    return Problem("Entity set 'RessourceDbContext.Ressources'  is null.");
                }
                var ressource = await _context.Resources.FindAsync(id);
                if (ressource != null)
                {
                    _context.Resources.Remove(ressource);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Home");
        }

        private bool RessourceExists(int id)
        {
            return _context.Resources.Any(e => e.ResourceID == id);
        }

    }
}
