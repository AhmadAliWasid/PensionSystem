#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Entities.Models;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class RestorationDemandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestorationDemandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RestorationDemands
        public async Task<IActionResult> Index()
        {
            return View(await _context.RestorationDemands.ToListAsync());
        }

        // GET: RestorationDemands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restorationDemand = await _context.RestorationDemands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restorationDemand == null)
            {
                return NotFound();
            }

            return View(restorationDemand);
        }

        // GET: RestorationDemands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RestorationDemands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Number,Date,IsPaid,PaymentDate")] RestorationDemand restorationDemand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restorationDemand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restorationDemand);
        }

        // GET: RestorationDemands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restorationDemand = await _context.RestorationDemands.FindAsync(id);
            if (restorationDemand == null)
            {
                return NotFound();
            }
            return View(restorationDemand);
        }

        // POST: RestorationDemands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Number,Date,IsPaid,PaymentDate")] RestorationDemand restorationDemand)
        {
            if (id != restorationDemand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restorationDemand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestorationDemandExists(restorationDemand.Id))
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
            return View(restorationDemand);
        }

        // GET: RestorationDemands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restorationDemand = await _context.RestorationDemands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restorationDemand == null)
            {
                return NotFound();
            }

            return View(restorationDemand);
        }

        // POST: RestorationDemands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restorationDemand = await _context.RestorationDemands.FindAsync(id);
            _context.RestorationDemands.Remove(restorationDemand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestorationDemandExists(int id)
        {
            return _context.RestorationDemands.Any(e => e.Id == id);
        }
    }
}