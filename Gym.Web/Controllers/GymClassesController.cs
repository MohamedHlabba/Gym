using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gym.Core.Entities;
using Gym.Data.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Gym.Web.Extensions;

namespace Gym.Web.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(ApplicationDbContext context , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

       // [Authorize(Roles ="Member")]
        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            var gymClass = await _context.GymClasses.Include(g => g.AttendingMembers).ThenInclude(u=>u.ApplicationUser)
                .ToListAsync();
              
            return View(gymClass);
        }
         [Authorize]
        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.Include(g=>g.AttendingMembers)
                .ThenInclude(g=>g.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }
        [Authorize]
        // GET: GymClasses/Create
        public IActionResult Create()
        {           
            return Request.IsAjax() ?  PartialView("CreatePartial") : View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                if (Request.IsAjax())
                {
                    
                    return PartialView("GymClassPartial", gymClass);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }
        [Authorize]
        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
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
        public ActionResult Fetch()
        {
            return PartialView("CreatePartial");
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,Duration,Description")] GymClass gymClass)
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
        [Authorize]
        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            _context.GymClasses.Remove(gymClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClasses.Any(e => e.Id == id);
        }
       // [Authorize]
        public async Task<IActionResult> BookingToogle(int? id)
        {

            if (id == null)
            {
                return NotFound();

            }
            var gymClass = await _context.GymClasses.Include(g => g.AttendingMembers)
               .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass.AttendingMembers == null)
            {
                gymClass.AttendingMembers = new List<ApplicationUserGymClass>();

            }
            var inloggadMember = userManager.GetUserId(User);

            var member = gymClass.AttendingMembers.Where(u => u.ApplicationUserId == inloggadMember)
                  .FirstOrDefault();
            if (member != null)
            {
                gymClass.AttendingMembers.Remove(member);
            }
            else
            {
                ApplicationUserGymClass newMember = new ApplicationUserGymClass();
                newMember.ApplicationUserId = inloggadMember;
                gymClass.AttendingMembers.Add(newMember);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Second approache 
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id is null)
                return BadRequest();
            var userId = userManager.GetUserId(User);
            var attending = await _context.ApplicationUserGyms.FindAsync(userId,id);
            if (attending is null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                };
                _context.ApplicationUserGyms.Add(booking);
            }
            else
            {
                _context.ApplicationUserGyms.Remove(attending);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
        public async Task<IActionResult> Booking()
        {
            var userId = userManager.GetUserId(User);
            var gympass =  _context.ApplicationUsers
                .Where(u => u.Id == userId).Where(u=>u.AttendedClasses.Count()!=0)
                .Select(u => u.AttendedClasses);

            //var gymp = _context.ApplicationUsers.Include(u=>u.AttendedClasses).ThenInclude(u=>u.ApplicationUser)
            //   .Where(u => u.AttendedClasses.)
            //   .Select(u => u.AttendedClasses);

            var gympasss = _context.ApplicationUserGyms
                .Where(u => u.ApplicationUserId == userId)
                .Select(u => u.GymClass);

            var gymClass = await _context.GymClasses.Include(g => g.AttendingMembers).ThenInclude(u => u.ApplicationUser)
                            .ToListAsync();
            var bookings = new List<GymClass>();


            var model = _context.ApplicationUserGyms
                .Where(u => u.ApplicationUserId == userId)
                .Select(a => a.GymClass);


            foreach (var item in gymClass)
            {
                if (item.AttendingMembers.Where(g => g.ApplicationUserId == userId).Count() != 0)
                {
                    bookings.Add(item);
                }

            }
            //var bookings = _context.GymClasses.Where(g => g.AttendingMembers == model.ToList());
            return View(nameof(Index),  bookings);
        }
    }

}
