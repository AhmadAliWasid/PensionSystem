using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class PensionPaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPensioner _pensioner;
        private readonly IMonthlyPensionDemand _monthlyPensionDemand;
        private readonly IPensionPayment _pensionPayment;
        private readonly ICompany _company;
        private readonly SessionHelper _sessionHelper;

        public PensionPaymentController(ApplicationDbContext applicationDbContext, IPensioner pensioner, IMonthlyPensionDemand monthlyPensionDemand,
            IPensionPayment pensionPayment, ICompany company, SessionHelper sessionHelper)
        {
            _context = applicationDbContext;
            _pensioner = pensioner;
            _monthlyPensionDemand = monthlyPensionDemand;
            _pensionPayment = pensionPayment;
            _company = company;
            _sessionHelper = sessionHelper;
        }

        public async Task<IActionResult> Home()
        {
            ViewData["DemandId"] = new SelectList(await _monthlyPensionDemand.GetOptions(DateTime.Now, _sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["CompanyId"] = new SelectList(await _company.GetOptions(), "Value", "Text");
            return View();
        }

        public async Task<IActionResult> Create()
        {
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["MonthlyPensionDemandId"] = new SelectList(await _monthlyPensionDemand.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            return View("_Create");
        }

        public async Task<IActionResult> Ledger()
        {
            ViewData["PensionerId"] = new SelectList(
                await _pensioner.GetOptions(_sessionHelper.GetUserPDUId())
                , "Value", "Text");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetConsolidatedByPensionerId(int PensionerId)
        {
            PensionerLedgerVM vm = new()
            {
                Pensioner = await _pensioner.Get(PensionerId),
                PensionerPayments = await _pensionPayment.GetByPensionerId(PensionerId),
                Session = new SessionViewModel()
                {
                    AMStamp = _sessionHelper.GetAMStamp(),
                    DMStamp = _sessionHelper.GetDMStamp(),
                    BaseStamp = _sessionHelper.GetBaseStamp(),
                }
            };
            return PartialView("_Ledger", vm);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateByDemandId(int monthlyPensionDemandId, int companyId)
        {
            var demand = await _monthlyPensionDemand.Get(monthlyPensionDemandId);
            if (demand == null)
                return NotFound("Error : Unable to Find Demand");
            var paymentList = await _pensionPayment.GetPensionPayments(monthlyPensionDemandId);
            if (paymentList != null)
            {
                var activeList = await _pensioner
                    .GetActiveByCompany(companyId, _sessionHelper.GetUserPDUId());
                if (activeList != null && activeList.Count > 0)
                {
                    var result = await _pensionPayment
                        .GenerateDemandList(activeList, monthlyPensionDemandId, demand.Date, companyId);
                    if (result == "Ok")
                        return await GetListByDemandId(monthlyPensionDemandId);
                    else
                        return NotFound(result);
                }
                else
                {
                    return PartialView("_List");
                }
            }
            else
                return await GetListByDemandId(monthlyPensionDemandId);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByDemandId(int monthlyPensionDemandId)
        {
            var list = await _pensionPayment.GetPensionPayments(monthlyPensionDemandId);

            if (list == null)
                return NotFound("No List of Found in this Demand");
            var Month = list.First().Month;

            Month = Month.AddMonths(-1);
            var lastMonthList = await _pensionPayment.GetByMonth(Month);
            var verified = list
                .Where(x => x.CertificateVerified == true && x.PhysicallyVerified == true);
            var notVerified = list
                .Where(x => x.CertificateVerified == false && x.PhysicallyVerified == false);
            PensionerPaymentViewModel model = new()
            {
                PayablesThisMonth = list,
                AmountVerified = verified.Sum(y => y.NetPension),
                AmountNonVerified = notVerified.Sum(y => y.NetPension),
                Verified = verified.Count(),
                NonVerified = notVerified.Count(),
                Month = list.First().Month,
                LastMonthList = lastMonthList
            };
            var bankVm = new List<BankDemandVM>();
            var bankList = list.Where(c => c.CertificateVerified == true)
                .GroupBy(x => x.Pensioner.Branch.Bank).ToList();
            foreach (var bank in bankList)
            {
                bankVm.Add(new BankDemandVM
                {
                    BankName = bank.Key.ShortName,
                    Amount = bank.Sum(y => y.NetPension),
                    Count = bank.Count()
                });
            }
            model.Session = new SessionViewModel()
            {
                AMStamp = _sessionHelper.GetAMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp(),
                DMStamp = _sessionHelper.GetDMStamp()
            };
            model.BankDemandVMs = bankVm;
            return PartialView("_List", model);
        }

        [HttpPost]
        public async Task<bool> VerifyCertificate(int Id = 0)
        {
            if (Id == 0)
                return false;
            return await _pensionPayment.VerifyCertificate(Id);
        }

        [HttpPost]
        public async Task<bool> VerifyPhysically(int? Id)
        {
            if (Id != null)
            {
                var payment = await _context.PensionerPayments.SingleOrDefaultAsync(x => x.Id == Id);
                if (payment != null)
                {
                    payment.PhysicallyVerified = true;
                    try
                    {
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.PensionerPayments.Include(x => x.Pensioner).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (model == null)
            {
                return NotFound();
            }
            return PartialView("Form", model);
        }

        [HttpPost]
        public async Task<JsonResult> Save(PensionerPayment model)
        {
            JsonResponseHelper jsonResponseHelper = new();
            try
            {
                var PPContext = _context.PensionerPayments;
                if (model.Id > 0)
                {
                    /// edit detail
                    if (PPContext != null)
                    {
                        var GetPayment = await PPContext.SingleOrDefaultAsync(x => x.Id == model.Id);
                        if (GetPayment != null)
                        {
                            GetPayment.MonthlyPension = model.MonthlyPension;
                            GetPayment.CMA = model.CMA;
                            GetPayment.OrderelyAllowence = model.OrderelyAllowence;
                            GetPayment.Deduction = model.Deduction;
                            GetPayment.NetPension = model.NetPension;
                            GetPayment.CertificateVerified = false;
                            await _context.SaveChangesAsync();
                            jsonResponseHelper.RCode = 1;
                            jsonResponseHelper.RText = "";
                        }
                        else
                        {
                            jsonResponseHelper.RCode = 0;
                            jsonResponseHelper.RText = "Not Found in Database";
                        }
                    }
                }
                else
                {
                    if (PPContext != null)
                    {
                        PPContext.Add(model);
                        await _context.SaveChangesAsync();
                        jsonResponseHelper.RCode = 1;
                    }
                }
            }
            catch (Exception exc)
            {
                jsonResponseHelper.RCode = 0;
                jsonResponseHelper.RText = ExceptionHelper.GetDetail(exc);
            }
            return Json(jsonResponseHelper);
        }
    }
}