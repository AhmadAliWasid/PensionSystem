#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Entities.Models;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class HBLRestorationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HBLRestorationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HBLRestorations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HBLRestorations.Include(h => h.Cheque).Include(h => h.Pensioner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HBLRestorations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hBLRestoration = await _context.HBLRestorations
                .Include(h => h.Cheque)
                .Include(h => h.Pensioner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hBLRestoration == null)
            {
                return NotFound();
            }

            return View(hBLRestoration);
        }

        // GET: HBLRestorations/Create
        public IActionResult Create()
        {
            ViewData["ChequeId"] = new SelectList(_context.Cheque, "Id", "Name");
            ViewData["PensionerId"] = new SelectList(_context.Pensioner, "Id", "AccountNumber");
            return View();
        }

        // POST: HBLRestorations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChequeId,PensionerId,Month,FromMonth,ToMonth,Amount,AccountNumber,CreatedDate,ModifiedDate")] HBLRestoration hBLRestoration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hBLRestoration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChequeId"] = new SelectList(_context.Cheque, "Id", "Name", hBLRestoration.ChequeId);
            ViewData["PensionerId"] = new SelectList(_context.Pensioner, "Id", "AccountNumber", hBLRestoration.PensionerId);
            return View(hBLRestoration);
        }

        // GET: HBLRestorations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hBLRestoration = await _context.HBLRestorations.FindAsync(id);
            if (hBLRestoration == null)
            {
                return NotFound();
            }
            ViewData["ChequeId"] = new SelectList(_context.Cheque, "Id", "Name", hBLRestoration.ChequeId);
            ViewData["PensionerId"] = new SelectList(_context.Pensioner, "Id", "AccountNumber", hBLRestoration.PensionerId);
            return View(hBLRestoration);
        }

        // POST: HBLRestorations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChequeId,PensionerId,Month,FromMonth,ToMonth,Amount,AccountNumber,CreatedDate,ModifiedDate")] HBLRestoration hBLRestoration)
        {
            if (id != hBLRestoration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hBLRestoration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HBLRestorationExists(hBLRestoration.Id))
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
            ViewData["ChequeId"] = new SelectList(_context.Cheque, "Id", "Name", hBLRestoration.ChequeId);
            ViewData["PensionerId"] = new SelectList(_context.Pensioner, "Id", "AccountNumber", hBLRestoration.PensionerId);
            return View(hBLRestoration);
        }

        // GET: HBLRestorations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hBLRestoration = await _context.HBLRestorations
                .Include(h => h.Cheque)
                .Include(h => h.Pensioner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hBLRestoration == null)
            {
                return NotFound();
            }

            return View(hBLRestoration);
        }

        // POST: HBLRestorations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hBLRestoration = await _context.HBLRestorations.FindAsync(id);
            _context.HBLRestorations.Remove(hBLRestoration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HBLRestorationExists(int id)
        {
            return _context.HBLRestorations.Any(e => e.Id == id);
        }
    }
}