using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using AutoMapper;
using PensionSystem.Entities.DTOs;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ChequesController(ApplicationDbContext context, ICheque cheque,
        IChequeCategory chequeCategory, IMapper mapper, SessionHelper sessionHelper, IHttpClientFactory httpClientFactory, IConfiguration configuration, ICommutation commutation, IHBLArrears hBLArrears) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICheque _cheque = cheque;
        private readonly IChequeCategory _chequeCategory = chequeCategory;
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly ICommutation _commutation = commutation;
        private readonly IHBLArrears _hblArrears = hBLArrears;
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
        [HttpGet]
        public async Task<IActionResult> GetPV(int Id)
        {
            var c = await _cheque.GetById(Id);
            if (c == null || c.ChequeCategory == null)
                return NotFound();
            var VM = new PVViewModel
            {
                Amount = c.Amount,
                Date = c.Date,
                Number = c.Number,
                Payee = c.Name,
                SessionViewModel = new SessionViewModel()
                {
                    AMStamp = _sessionHelper.GetAMStamp(),
                    BaseStamp = _sessionHelper.GetBaseStamp(),
                    DMStamp = _sessionHelper.GetDMStamp(),
                }
            };
            if (c.ChequeCategoryId == 1)
            {
                VM.Description.Add(new(c.ChequeCategory.Name + " for the month of " + c.Date.ToString("MM-yyyy"), c.Amount));
            }
            else
            {
                // we will add commutation 
                var commutation = await _commutation.GetCommutationByCheque(c.Id);
                if (commutation != null && commutation.Count > 0)
                    VM.Description.Add(new(" Commutation ", commutation.Sum(x => x.Amount)));
                // we will cheque if arrears
                var arrears = await _hblArrears.GetArrearsByCheque(c.Id);
                if (arrears.Count > 0)
                {
                    foreach (var item in arrears)
                    {

                        VM.Description.Add(new(item.Description, c.Amount));
                        VM.Description.Add(new($"Period ({UserFormat.GetDate(item.FromMonth)} {UserFormat.GetDate(item.ToMonth)}) ", 0));
                    }
                }

            }
            return PartialView("_PV", VM);

        }
    }
}