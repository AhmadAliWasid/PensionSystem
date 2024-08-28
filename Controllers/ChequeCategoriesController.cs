using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Entities.Models;

namespace PensionSystem.Controllers
{
    public class ChequeCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChequeCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChequeCategories
        public async Task<IActionResult> Index()
        {
            var ListChequeCategories = _context.ChequeCategory;
            return ListChequeCategories != null ? View(await ListChequeCategories.ToListAsync()) : View();
        }

        // GET: ChequeCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chequeCategory = await _context.ChequeCategory.FirstOrDefaultAsync(m => m.Id == id);
            if (chequeCategory == null)
            {
                return NotFound();
            }

            return View(chequeCategory);
        }

        // GET: ChequeCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChequeCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CreatedDate,ModifiedDate")] ChequeCategory chequeCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chequeCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chequeCategory);
        }

        // GET: ChequeCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chequeCategory = await _context.ChequeCategory.FindAsync(id);
            if (chequeCategory == null)
            {
                return NotFound();
            }
            return View(chequeCategory);
        }

        // POST: ChequeCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedDate,ModifiedDate")] ChequeCategory chequeCategory)
        {
            if (id != chequeCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chequeCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChequeCategoryExists(chequeCategory.Id))
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
            return View(chequeCategory);
        }

        // GET: ChequeCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chequeCategory = await _context.ChequeCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chequeCategory == null)
            {
                return NotFound();
            }

            return View(chequeCategory);
        }

        // POST: ChequeCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ChequeCategory? chequeCategory = await _context.ChequeCategory.FindAsync(id);
            if (chequeCategory != null)
            {
                _context.ChequeCategory.Remove(chequeCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        private bool ChequeCategoryExists(int id)
        {
            return _context.ChequeCategory.Any(e => e.Id == id);
        }
    }
}