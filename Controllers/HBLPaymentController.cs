using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using System.Data;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class HBLPaymentController(
        ApplicationDbContext _context,
        IHBLArrears _hBLArrears,
        IHBLPayments _hBLPayments,
        ICommutation _commutation,
        ICheque _cheque,
        IMonthlyPensionDemand _monthlyPensionDemand,
        IPensionPayment _pensionPayment,
        IPensioner _pensioner,
        IBank _bank,
        IBranch _branch,
        ICashBook _cashBook,
        SessionHelper _sessionHelper) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var pduId = _sessionHelper.GetUserPDUId();
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(1, pduId), "Value", "Text");
            ViewData["DemandId"] = new SelectList(await _monthlyPensionDemand.GetOptions(pduId), "Value", "Text");
            ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text");
            return View(new PensionerPaymentViewModel());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var pduId = _sessionHelper.GetUserPDUId();
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(1, pduId), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(pduId, true), "Value", "Text");
            ViewData["BranchId"] = new SelectList(await _branch.GetOptions(), "Value", "Text");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HBLPayment hBLPayment)
        {
            try
            {
                if (_context.HBLPayments is null)
                    return View("Error");
                hBLPayment.CreatedDate = hBLPayment.ModifiedOn = DateTime.Now;
                await _context.HBLPayments.AddAsync(hBLPayment);
                await _context.SaveChangesAsync();
               
                return await GetByChequeId(hBLPayment.ChequeId);
            }
            catch (Exception exc)
            {
                return View("Error", new ErrorViewModel { RequestId = exc.Message });
            }
        }

        public IActionResult History() => View();

        public async Task<IActionResult> GetList(DateTime month)
        {
            int onlyMonth = month.Month, onlyYear = month.Year;
            var hblList = await _context.HBLPayments
                .Include(p => p.Pensioner)
                .Where(x => x.Month.Month == onlyMonth && x.Month.Year == onlyYear)
                .OrderBy(y => y.Pensioner.PPOSystem)
                .ToListAsync();

            if (hblList.Count == 0)
            {
                var payments = await _context.PensionerPayments
                    .Include(y => y.Pensioner)
                    .Where(x => x.Month.Month == onlyMonth && x.Month.Year == onlyYear && x.NetPension >= 1)
                    .ToListAsync();

                foreach (var item in payments)
                {
                    _context.HBLPayments.Add(new HBLPayment
                    {
                        PensionerId = item.PensionerId,
                        AccountNumber = item.Pensioner.AccountNumber,
                        MonthlyPension = item.MonthlyPension,
                        CMA = item.CMA,
                        OrderlyAllowance = item.OrderelyAllowence,
                        Deduction = item.Deduction,
                        Total = item.NetPension,
                        Month = item.Month,
                        CreatedDate = DateTime.Now,
                        ModifiedOn = DateTime.Now
                    });
                }
                try
                {
                    await _context.SaveChangesAsync();
                    hblList = await _context.HBLPayments
                        .Include(p => p.Pensioner)
                        .Where(x => x.Month.Month == onlyMonth && x.Month.Year == onlyYear)
                        .OrderBy(z => z.Pensioner.PPOSystem)
                        .ToListAsync();
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("Error", exc.Message);
                    return PartialView("_List", new HBLPaymentViewModel());
                }
            }

            decimal totalPaid = hblList.Sum(x => x.Total);
            var cheque = await _cheque.GetCheque(totalPaid, month);
            var session = new SessionViewModel
            {
                DMStamp = _sessionHelper.GetDMStamp(),
                AMStamp = _sessionHelper.GetAMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp(),
                PDUId = _sessionHelper.GetUserPDUId()
            };
            var model = new HBLPaymentViewModel
            {
                CurrentMonth = month,
                HBLPaymentsList = hblList,
                TotalMP = hblList.Sum(x => x.MonthlyPension),
                TotalCMA = hblList.Sum(x => x.CMA),
                TotalOrderly = hblList.Sum(x => x.OrderlyAllowance),
                TotalDeduction = hblList.Sum(x => x.Deduction),
                TotalPaid = totalPaid,
                ChequeDate = cheque.Date,
                ChequeNumber = cheque.Number,
                ChequeAmount = cheque.Amount,
                Session = session,
                HBLArrears = await _hBLArrears.GetArrearsAsync(new DateOnly(month.Year, month.Month, month.Day))
            };
            return PartialView("_List", model);
        }

        public async Task<IActionResult> Consolidated()
        {
            ViewData["PensionerId"] = new SelectList(
                await _pensioner.GetOptions(_sessionHelper.GetUserPDUId(), false), "Value", "Text");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PPOPage()
        {
            ViewData["PensionerId"] = new SelectList(
                await _pensioner.GetOptions(_sessionHelper.GetUserPDUId(), false), "Value", "Text");
            return View("_PPOPage");
        }

        [HttpPost]
        public async Task<IActionResult> GetConsolidated(DateTime StartingMonth, DateTime EndingMonth)
        {
            if (EndingMonth < StartingMonth)
                return NotFound("Starting Date must be before the ending date ");

            DateTime start = new(StartingMonth.Year, StartingMonth.Month, 1);
            DateTime end = EndingMonth.AddMonths(1).AddDays(-1);
            var pduId = _sessionHelper.GetUserPDUId();

            var listHblPayments = await _hBLPayments.GetConsolidatedPensioner(start, end, pduId);
            var model = new ConsolidatedSummaryModel
            {
                Pensioners = listHblPayments?.GroupBy(x => x.Pensioner.Id)
                .Select(g => g.First())
                .OrderBy(z => z.Pensioner.PPOSystem).ToList(),
                Months = listHblPayments?.GroupBy(x => x.Month.Month)
                .Select(g => g.First())
                .OrderBy(z => z.Month).ToList(),
                HBLPayments = listHblPayments,
                HBLArrears = await _hBLArrears.GetArrearsByDates(start, end, pduId),
                AllPensioners = (await _hBLPayments.GetAllPensioners(start, end, pduId))?.OrderBy(x => x.PPOSystem).ToList(),
                Commutations = await _commutation.GetCommutationsByDates(start, end, pduId),
                BankCharges = await _cashBook.GetBankChargesBetweenMonths(start, end, pduId),
                Session = new SessionViewModel
                {
                    AMStamp = _sessionHelper.GetAMStamp(),
                    DMStamp = _sessionHelper.GetDMStamp(),
                    BaseStamp = _sessionHelper.GetBaseStamp()
                }
            };
            return PartialView("_ConsolidatedSummary", model);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            var context = _context.HBLPayments;
            var jsonResponse = new JsonResponseHelper();
            if (context != null)
            {
                var item = await context.FirstOrDefaultAsync(x => x.Id == Id);
                if (item != null)
                {
                    try
                    {
                        context.Remove(item);
                        await _context.SaveChangesAsync();
                        jsonResponse.RCode = 1;
                    }
                    catch (Exception exc)
                    {
                        jsonResponse.RCode = 0;
                        jsonResponse.RText = exc.Message;
                    }
                }
            }
            else
            {
                jsonResponse.RCode = 0;
                jsonResponse.RText = "Entity error";
            }
            return Json(jsonResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetByChequeId(int ChequeId) =>
            await GetChequeViewModel(ChequeId, "_List");

        [HttpPost]
        public async Task<JsonResponseHelper> SendSMSByChequeId(int ChequeId)
        {
            var list = await _hBLPayments.GetByChequeId(ChequeId);
            if (list is { Count: > 0 })
            {
                var listMessage = new List<MessagesHistory>();
                foreach (var item in list)
                {
                    if (item.Pensioner?.IsServiceActive == true)
                    {
                        var mobile = item.Pensioner.Mobile.Replace("-", "").Replace("+", "");
                        var account = $"{item.Branch.Bank.ShortName} ({item.Branch.Code}-{item.AccountNumber})";
                        var amount = UserFormat.GetAmount(item.Total);
                        var message = $"Dear {item.Pensioner.Claimant}!, your Pension Rs ={amount}/- has been successfully sent to your bank {account} Please kindly check it after one hour.";
                        var jsonResult = await SMSSender.SendSMS(mobile, message);
                        listMessage.Add(new MessagesHistory
                        {
                            MessageText = message,
                            PensionerId = item.PensionerId,
                            ToNumber = mobile,
                            JsonResponse = jsonResult
                        });
                    }
                }
                await _context.MessagesHistories.AddRangeAsync(listMessage);
                await _context.SaveChangesAsync();
                return new JsonResponseHelper { RCode = 1, RText = "Sent Successfully" };
            }
            return new JsonResponseHelper { RCode = 0, RText = "No Pensioner In List" };
        }

        [HttpGet]
        public async Task<IActionResult> GetChequeList(int ChequeId) =>
            await GetChequeViewModel(ChequeId, "_ChequeList");

        [HttpPost]
        public async Task<IActionResult> PayByChequeNow(int ChequeId, int DemandId, int BankId)
        {
            if (ChequeId == 0 || DemandId == 0 || BankId == 0)
                return NotFound("Invalid Cheque or Demand or bank");

            var cheque = await _cheque.GetCheque(ChequeId);
            if (cheque == null)
                return NotFound("Unable to find to cheque");

            var listPensionPayment = await _pensionPayment.GetPensionPayments(DemandId);
            if (listPensionPayment == null)
                return NotFound("Empty Demand");

            decimal demandAmount = listPensionPayment
                .Where(x => x.Pensioner.Branch.BankId == BankId && x.CertificateVerified && x.PhysicallyVerified)
                .Sum(x => x.NetPension);

            if (demandAmount > cheque.Amount)
                return NotFound($"Bank Amount of the selected Demand  can not be greater then Cheque Amount <br> Demand : {UserFormat.GetAmount(demandAmount)} ");

            var result = await _hBLPayments.PayDemandByCheque(ChequeId, DemandId, BankId);
            if (result != "Ok")
                return NotFound(result);

            return await GetByChequeId(ChequeId);
        }

        [HttpGet]
        public async Task<IActionResult> GetConsolidatedByPensionerId(int PensionerId) =>
            PartialView("_PensionerHistory", new HBLPaymentPensionerHistoryVM
            {
                HBLPayments = await _hBLPayments.GetByPensionerId(PensionerId),
                Pensioner = await _pensioner.Get(PensionerId)
            });

        // Helper to reduce code duplication for cheque-based views
        private async Task<IActionResult> GetChequeViewModel(int ChequeId, string partialView)
        {
            var list = await _hBLPayments.GetByChequeId(ChequeId);
            if (list is null || list.Count == 0)
                return NotFound("No Data Found!");

            var cheque = await _cheque.GetCheque(ChequeId);
            var vm = new HBLPaymentViewModel
            {
                HBLPaymentsList = list,
                CurrentMonth = list.First().Month,
                ChequeNumber = cheque.Number,
                ChequeDate = cheque.Date,
                ChequeAmount = cheque.Amount,
                ChequePayee = cheque.Name,
                TotalMP = list.Sum(h => h.MonthlyPension),
                TotalCMA = list.Sum(h => h.CMA),
                TotalOrderly = list.Sum(h => h.OrderlyAllowance),
                TotalDeduction = list.Sum(h => h.Deduction),
                TotalPaid = list.Sum(x => x.Total),
                Session = new SessionViewModel
                {
                    AMStamp = _sessionHelper.GetAMStamp(),
                    DMStamp = _sessionHelper.GetDMStamp(),
                    BaseStamp = _sessionHelper.GetBaseStamp(),
                    PDUId = _sessionHelper.GetUserPDUId()
                }
            };
            return PartialView(partialView, vm);
        }
    }
}