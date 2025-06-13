using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Entities.Models;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using WebAPI.ViewModels;
using WebAPI.Helpers;
namespace WebAPI.Controllers
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
        private readonly IMapper _mapper;
        private readonly ICommutation _commutation;
        private readonly IHBLArrears _hblArrears;

        public ChequesController(
            ApplicationDbContext context,
            ICheque cheque,
            IChequeCategory chequeCategory,
            IMapper mapper,
            SessionHelper sessionHelper,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ICommutation commutation,
            IHBLArrears hblArrears)
        {
            _context = context;
            _cheque = cheque;
            _chequeCategory = chequeCategory;
            _sessionHelper = sessionHelper;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _mapper = mapper;
            _commutation = commutation;
            _hblArrears = hblArrears;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Crud(int id)
        {
            ViewData["ChequeCategoryId"] = new SelectList(await _chequeCategory.GetOptions(), "Value", "Text");
            if (id == 0)
            {
                var cheque = new Cheque
                {
                    Number = await _cheque.GetChequeNumber(_sessionHelper.GetUserPDUId())
                };
                return PartialView("_Crud", cheque);
            }
            else
            {
                var cheque = await _cheque.GetById(id);
                if (cheque == null)
                    return NotFound();
                ViewData["ChequeCategoryId"] = new SelectList(await _chequeCategory.GetOptions(), "Value", "Text", cheque.ChequeCategoryId);
                return PartialView("_Crud", cheque);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Save(Cheque cheque)
        {
            var jsonResponseHelper = new JsonResponseHelper();
            if (!ModelState.IsValid)
            {
                jsonResponseHelper.RText = "Invalid data.";
                return Json(jsonResponseHelper);
            }

            cheque.PDUId = _sessionHelper.GetUserPDUId();
            try
            {
                if (cheque.Id == 0)
                {
                    var result = await _cheque.Insert(cheque);
                    if (result.IsSaved)
                        jsonResponseHelper.RCode = 1;
                }
                else
                {
                    var result = await _cheque.Update(cheque);
                    if (result.IsSaved)
                        jsonResponseHelper.RCode = 1;
                }
            }
            catch (Exception exc)
            {
                jsonResponseHelper.RText = exc.Message;
            }
            return Json(jsonResponseHelper);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var cheque = await _context.Cheque
                .Include(c => c.ChequeCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cheque == null)
                return NotFound();

            return View(cheque);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cheque = await _context.Cheque.FindAsync(id);
            if (cheque == null)
                return NotFound();

            _context.Cheque.Remove(cheque);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> MarkItPaid(int id)
        {
            if (id == 0)
                return Json(false);

            var result = await _cheque.MarkItPay(id);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Load()
        {
            var cheques = await _cheque.GetList(_sessionHelper.GetUserPDUId());
            return PartialView("_List", cheques);
        }

        [HttpGet]
        public async Task<IActionResult> GetPV(int id)
        {
            var c = await _cheque.GetById(id);
            if (c == null || c.ChequeCategory == null)
                return NotFound();

            var vm = new PVViewModel
            {
                Amount = c.Amount,
                Date = c.Date,
                Number = c.Number,
                Payee = c.Name,
                SessionViewModel = new SessionViewModel
                {
                    AMStamp = _sessionHelper.GetAMStamp(),
                    BaseStamp = _sessionHelper.GetBaseStamp(),
                    DMStamp = _sessionHelper.GetDMStamp(),
                }
            };

            if (c.ChequeCategoryId == 1)
            {
                vm.Description.Add(new(c.ChequeCategory.Name + " for the month of " + c.Date.ToString("MM-yyyy"), c.Amount));
            }
            else
            {
                var commutation = await _commutation.GetCommutationByCheque(c.Id);
                if (commutation != null && commutation.Count > 0)
                    vm.Description.Add(new("Commutation", commutation.Sum(x => x.Amount)));

                var arrears = await _hblArrears.GetArrearsByCheque(c.Id);
                if (arrears.Count > 0)
                {
                    foreach (var item in arrears)
                    {
                        vm.Description.Add(new(item.Description, item.Amount));
                        vm.Description.Add(new($"Period ({UserFormat.GetDate(item.FromMonth)} {UserFormat.GetDate(item.ToMonth)})", 0));
                    }
                }
            }
            return PartialView("_PV", vm);
        }
    }
}