using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class HBLArrearsController(ApplicationDbContext context, IHBLArrears hBLArrears,
        IPensioner pensioner,
        ICheque cheque,
        IArrearsDemand arrearsDemand, IArrearsPayment arrearsPayment,
        IBranch branch, SessionHelper sessionHelper, IBank bank) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHBLArrears _hblArrears = hBLArrears;
        private readonly IPensioner _pensioner = pensioner;
        private readonly ICheque _cheque = cheque;
        private readonly IArrearsDemand _arrearsDemand = arrearsDemand;
        private readonly IArrearsPayment _arrearsPayment = arrearsPayment;
        private readonly IBranch _branch = branch;
        private readonly IBank _bank = bank;
        private readonly SessionHelper _sessionHelper = sessionHelper;

        // GET: HBLArrears
        public async Task<IActionResult> Index()
        {
            HBLArrearsViewModel model = new()
            {
                HBLArrears = await _hblArrears.Get()
            };
            ViewData["ArrearsDemandId"] = new SelectList(await _arrearsDemand.GetUnpaidOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptionsUnpaid(2, _sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text");
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var hBLArrears = await _context.HBLArrears
                .Include(h => h.Cheque)
                .Include(h => h.Pensioner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hBLArrears == null)
            {
                return NotFound();
            }

            return View(hBLArrears);
        }

        // GET: HBLArrears/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptionsUnpaidTwoCats(2, 3, _sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["BranchId"] = new SelectList(await _branch.GetOptions(), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(HBLArrears hBLArrears)
        {
            var jsH = new JsonResponseHelper();
            if (ModelState.IsValid)
            {
                var cheque = await _cheque.GetCheque(hBLArrears.ChequeId);
                if(cheque == null)
                {
                    jsH.RCode = 0;
                    jsH.RText = "Cheque not found!";
                    return Json(jsH);
                }
                hBLArrears.Month = cheque.Date;
                try
                {
                    _context.Add(hBLArrears);
                    await _context.SaveChangesAsync();
                    return Json(new JsonResponseHelper { RCode = 1, RText = "Saved Successfully" });
                }
                catch (Exception exc)
                {
                    return Json(new JsonResponseHelper { RCode = 0, RText = ExceptionHelper.GetDetail(exc) });
                }
            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                jsH.RCode = 0;
                foreach (var item in allErrors)
                {
                    jsH.RText += item.ErrorMessage + "</br>";
                }
                return Json(jsH);
            }
        }

        // GET: HBLArrears/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hBLArrears = await _context.HBLArrears.FindAsync(id);
            if (hBLArrears == null)
            {
                return NotFound();
            }
            ViewData["BranchId"] = new SelectList(await _branch.GetOptions(), "Value", "Text", hBLArrears.BranchId);
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text", hBLArrears.ChequeId);
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text", hBLArrears.PensionerId);
            return View(hBLArrears);
        }

        // POST: HBLArrears/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChequeId,PensionerId,BranchId,Month,Description,AccountNumber,FromMonth,ToMonth,Amount,CreatedDate,ModifiedDate")] HBLArrears hBLArrears)
        {
            if (id != hBLArrears.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hBLArrears);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HBLArrearsExists(hBLArrears.Id))
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
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            return View(hBLArrears);
        }

        // GET: HBLArrears/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hBLArrears = await _context.HBLArrears
                .Include(h => h.Cheque)
                .Include(h => h.Pensioner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hBLArrears == null)
            {
                return NotFound();
            }

            return View(hBLArrears);
        }

        // POST: HBLArrears/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hBLArrears = await _context.HBLArrears.FindAsync(id);
            if (hBLArrears != null)
            {
                _context.HBLArrears.Remove(hBLArrears);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HBLArrearsExists(int id)
        {
            return _context.HBLArrears.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetCheque(int chequeId)
        {
            HBLArrearsViewModel hBLArrearsViewModel = new()
            {
                HBLArrears = await _hblArrears.GetArrearsByCheque(chequeId),
                Cheque = await _cheque.GetCheque(chequeId),
                Session = new SessionViewModel()
                {
                    AMStamp = _sessionHelper.GetAMStamp(),
                    DMStamp = _sessionHelper.GetDMStamp(),
                    BaseStamp = _sessionHelper.GetBaseStamp(),
                }
            };
            return PartialView("_List", hBLArrearsViewModel);
        }

        [HttpPost]
        public async Task<JsonResult> PayArrearDemand(int ArrearDemandId, int ChequeId, int BankId)
        {
            JsonResponseHelper jsonResponseHelper = new();
            if (ArrearDemandId != 0 && ChequeId != 0 && BankId != 0)
            {
                var listArrearsPayment = await _arrearsPayment.GetByDemandForPayment(ArrearDemandId, BankId);
                var cheque = await _cheque.GetCheque(ChequeId);
                if (cheque != null)
                {
                    string result = await _hblArrears.PayHBLArrearsInBulk(listArrearsPayment, ChequeId, cheque.Date);
                    jsonResponseHelper.RText = result;
                    if (result == string.Empty)
                        jsonResponseHelper.RCode = 0;
                    else
                        jsonResponseHelper.RCode = 1;
                }   
            }
            else
            {
                jsonResponseHelper.RCode = 1;
                jsonResponseHelper.RText = "Demand or Cheque is not provided!";
            }
            return Json(jsonResponseHelper);
        }
    }
}