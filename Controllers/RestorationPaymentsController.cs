#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Helpers;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class RestorationPaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPensioner _pensioner;
        private readonly IRestorationDemand _restorationDemand;
        private readonly IRestorationPayment _restorationPayment;
        private readonly SessionHelper _sessionHelper;

        public RestorationPaymentsController(ApplicationDbContext context,
            IPensioner pensioner, IRestorationDemand restorationDemand, IRestorationPayment restorationPayment, SessionHelper sessionHelper)
        {
            _context = context;
            _pensioner = pensioner;
            _restorationDemand = restorationDemand;
            _restorationPayment = restorationPayment;
            _sessionHelper = sessionHelper;
        }

        // GET: RestorationPayments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RestorationPayments.Include(r => r.RestorationDemand)
                .Include(r => r.Pensioner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RestorationPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var restorationPayment = await _context.RestorationPayments
                .Include(r => r.RestorationDemand)
                .Include(r => r.Pensioner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restorationPayment == null)
            {
                return NotFound();
            }
            return View(restorationPayment);
        }

        // GET: RestorationPayments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["RestorationDemandId"] = new SelectList(await _restorationDemand.GetOptions(), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            return View();
        }

        // POST: RestorationPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([Bind("PensionerId,FromMonth,ToMonth,NumberOfMonths,MP,CMA,Orderly,NetPension,Total,Month,,RestorationDemandId,Description")]
        RestorationPayment restorationPayment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(restorationPayment);
                    await _context.SaveChangesAsync();
                    return Json(new JsonResponseHelper { RCode = 1, RText = "" });
                }
                catch (Exception exc)
                {
                    return Json(new JsonResponseHelper { RCode = 1, RText = ExceptionHelper.GetDetail(exc) });
                }
            }
            else
            {
                return Json(new JsonResponseHelper { RCode = 0, RText = "" });
            }
        }

        // GET: RestorationPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restorationPayment = await _context.RestorationPayments.FindAsync(id);
            if (restorationPayment == null)
            {
                return NotFound();
            }
            ViewData["ArrearsDemandId"] = new SelectList(_context.ArrearsDemands, "Id", "Description", restorationPayment.RestorationDemandId);
            ViewData["PensionerId"] = new SelectList(_context.Pensioner, "Id", "AccountNumber", restorationPayment.PensionerId);
            return View(restorationPayment);
        }

        // POST: RestorationPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PensionerId,FromMonth,ToMonth,NumberOfMonths,MP,CMA,Orderly,NetPension,Total,Month,CreatedDate,ArrearsDemandId,Description")] RestorationPayment restorationPayment)
        {
            if (id != restorationPayment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restorationPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestorationPaymentExists(restorationPayment.Id))
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
            ViewData["ArrearsDemandId"] = new SelectList(_context.ArrearsDemands, "Id", "Description", restorationPayment.RestorationDemandId);
            ViewData["PensionerId"] = new SelectList(_context.Pensioner, "Id", "AccountNumber", restorationPayment.PensionerId);
            return View(restorationPayment);
        }

        // GET: RestorationPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restorationPayment = await _context.RestorationPayments
                .Include(r => r.RestorationDemand)
                .Include(r => r.Pensioner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restorationPayment == null)
            {
                return NotFound();
            }

            return View(restorationPayment);
        }

        // POST: RestorationPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restorationPayment = await _context.RestorationPayments.FindAsync(id);
            _context.RestorationPayments.Remove(restorationPayment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestorationPaymentExists(int id)
        {
            return _context.RestorationPayments.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetDemandList(int demandId)
        {
            RestorationPaymentVM model = new()
            {
                RestorationDemand = await _restorationDemand.GetRestorationDemand(demandId),
                RestorationPayments = await _restorationPayment.GetRestorationPayments(demandId)
            };
            return PartialView("_DemandListPensioner", model);
        }
    }
}