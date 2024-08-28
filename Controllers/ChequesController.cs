using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ChequesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICheque _cheque;
        private readonly IChequeCategory _chequeCategory;
        private readonly SessionHelper _sessionHelper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public ChequesController(ApplicationDbContext context, ICheque cheque,
            IChequeCategory chequeCategory, SessionHelper sessionHelper, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _cheque = cheque;
            _chequeCategory = chequeCategory;
            _sessionHelper = sessionHelper;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: Cheques
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                ViewData["ChequeCategoryId"] = new SelectList(await _chequeCategory.GetOptions(), "Value", "Text");
                var r = new Cheque
                {
                    Number = await _cheque.GetChequeNumber(_sessionHelper.GetUserPDUId())
                };
                return PartialView("_Crud", r);
            }
            else
            {
                var r = await _cheque.GetById(id);
                ViewData["ChequeCategoryId"] = new SelectList(await _chequeCategory.GetOptions(), "Value", "Text", r.ChequeCategoryId);
                return PartialView("_Crud", r);
            }
        }
        [HttpPost]
        public async Task<JsonResult> Save(Cheque cheque)
        {
            JsonResponseHelper jsonResponseHelper = new();
            if (ModelState.IsValid)
            {
                cheque.PDUId = _sessionHelper.GetUserPDUId();
                try
                {
                    if (cheque.Id == 0)
                    {
                        var r = await _cheque.Insert(cheque);
                        if (r.IsSaved)
                            jsonResponseHelper.RCode = 1;
                    }
                    else
                    {
                        var r = await _cheque.Update(cheque);
                        if (r.IsSaved)
                            jsonResponseHelper.RCode = 1;
                    }
                }
                catch (Exception exc)
                {
                    jsonResponseHelper.RText = exc.Message.ToString();
                }
            }
            return Json(jsonResponseHelper);
        }

        // GET: Cheques/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cheque = await _context.Cheque
                .Include(c => c.ChequeCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cheque == null)
            {
                return NotFound();
            }

            return View(cheque);
        }

        // POST: Cheques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Cheque? cheque = await _context.Cheque.FindAsync(id);
            if (cheque != null)
            {
                _context.Cheque.Remove(cheque);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound(); ;
            }
        }

        [HttpPost]
        public async Task<bool> MarkItPaid(int Id = 0)
        {
            if (Id == 0)
                return false;
            return await _cheque.MarkItPay(Id);
        }
        [HttpGet]
        public async Task<IActionResult> Load()
        {
            return PartialView("_List",
                await _cheque.GetList(_sessionHelper.GetUserPDUId()));
        }
    }
}