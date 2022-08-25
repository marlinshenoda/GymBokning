using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBokning.Data;
using GymBokning.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GymBokning.Models.ModelView;
using Microsoft.AspNetCore.Identity;

namespace GymBokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;

        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            return _context.GymClasses != null ?
                        View(await _context.GymClasses.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.GymClasses'  is null.");
        }
        public async Task<IActionResult> BookedGymClassAllList()
        {
            var model = await GetModel();

            //all gymclasses regardless of StartTime
            return View("BookedGymClassAllListView", model);
        }
        public ActionResult HistoryToAllGymClasses() => RedirectToAction("BookedGymClassAllList");

        public async Task<IEnumerable<BookedGymClassViewModel>> GetModel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _context.GymClasses.Select(i => new BookedGymClassViewModel
            {
                Id = i.Id,
                Name = i.Name,
                StartTime = i.StartTime,
                Duration = i.Duration,
                Description = i.Description,
                ApplicationUserGymClassIsBooked = i.AttendingMembers.Any(a => a.ApplicationUserId == userId)
            }).ToListAsync();

            var modelOrdered = model.OrderByDescending(m => m.StartTime);
            return modelOrdered;
        }
        public ActionResult HistoryToNewGymClasses() => RedirectToAction("BookedGymClass");

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymClasses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GymClasses'  is null.");
            }
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return (_context.GymClasses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

 
        [Authorize]

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null) return NotFound();

            else
            {
                //this user is logged in             
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //which class?
                var gymClass = await _context.GymClasses.FirstOrDefaultAsync(g => g.Id == id);

                //logged user is on this class 
                var b = await _context.ApplicationUserGymClass.FirstOrDefaultAsync(t => t.ApplicationUserId == userId && t.GymClassId == id);

                //user is not on this class: add user to this class (add into ApplicationUserGymClass db)
                if (b == null)
                {
                    var classAndMember = new ApplicationUserGymClass
                    {
                        ApplicationUser = _context.Users.ToList().FirstOrDefault(u => u.Id == userId),
                        GymClass = gymClass,
                    };
                    _context.ApplicationUserGymClass.Add(classAndMember);
                }
                else
                {
                    _context.ApplicationUserGymClass.Remove(b);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { id = gymClass.Id });
            }
        }
    }
}
