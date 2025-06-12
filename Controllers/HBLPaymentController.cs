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
using System.Data;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class HBLPaymentController(ApplicationDbContext applicationDbContext,
        IHBLArrears hBLArrearsService, IHBLPayments hBLPayments,
        ICommutation commutation,
        ICheque cheque,
        IMonthlyPensionDemand monthlyPensionDemand,
        IPensionPayment pensionPayment,
        IPensioner pensioner,
        IBank bank,
        IBranch branch,
        ICashBook cashBook,
        SessionHelper sessionHelper) : Controller
    {
        private readonly ApplicationDbContext _context = applicationDbContext;
        private readonly IHBLArrears _hBLArrears = hBLArrearsService;
        private readonly IHBLPayments _hBLPayments = hBLPayments;
        private readonly ICommutation _commutation = commutation;
        private readonly ICheque _cheque = cheque;
        private readonly IMonthlyPensionDemand _monthlyPensionDemand = monthlyPensionDemand;
        private readonly IPensionPayment _pensionPayment = pensionPayment;
        private readonly IPensioner _pensioner = pensioner;
        private readonly IBank _bank = bank;
        private readonly IBranch _branch = branch;
        private readonly ICashBook _cashBook = cashBook;
        private readonly SessionHelper _sessionHelper = sessionHelper;

        public async Task<IActionResult> Index()
        {
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(1, _sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["DemandId"] = new SelectList(await _monthlyPensionDemand.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text");
            PensionerPaymentViewModel vm = new();
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(1, _sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId(), true), "Value", "Text");
            ViewData["BranchId"] = new SelectList(await _branch.GetOptions(), "Value", "Text");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HBLPayment hBLPayment)
        {
            try
            {
                var myContext = _context;
                var hContext = myContext.HBLPayments;
                if (hContext == null)
                    return View("Error");
                hBLPayment.CreatedDate = DateTime.Now;
                hBLPayment.ModifiedOn = DateTime.Now;
                await hContext.AddAsync(hBLPayment);
                await myContext.SaveChangesAsync();
                return await GetByChequeId(hBLPayment.ChequeId);
            }
            catch (Exception exc)
            {
                ErrorViewModel EVM = new()
                {

                    RequestId = exc.Message.ToString()
                };
                return View("Error", EVM);
            }
        }

        public IActionResult History()
        {
            return View();
        }

        public async Task<IActionResult> GetList(DateTime month)
        {
            int onlyMonth = month.Month;
            int onlyYear = month.Year;
            var hblInCurrentMontList = await _context.HBLPayments.Include(p => p.Pensioner).Where(x => x.Month.Month == onlyMonth && x.Month.Year == onlyYear).OrderBy(y => y.Pensioner.PPOSystem).ToListAsync();
            HBLPaymentViewModel model = new();
            if (hblInCurrentMontList.Count == 0)
            {
                var PaymentList = await _context.PensionerPayments.Include(y => y.Pensioner).Where(x => x.Month.Month == onlyMonth && x.Month.Year == onlyYear).ToListAsync();
                var FilteredPaymentList = PaymentList.Where(x => x.NetPension >= 1);
                foreach (var item in FilteredPaymentList)
                {

                    HBLPayment hBLPayment = new()
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
                    };
                    _context.HBLPayments.Add(hBLPayment);
                }
                try
                {
                    await _context.SaveChangesAsync();
                    hblInCurrentMontList = await _context.HBLPayments.Include(p => p.Pensioner).Where(x => x.Month.Month == onlyMonth && x.Month.Year == onlyYear).OrderBy(z => z.Pensioner.PPOSystem).ToListAsync();
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("Error", exc.Message.ToString());
                    return PartialView("_List", model);
                }
            }
            model.CurrentMonth = month;
            model.HBLPaymentsList = hblInCurrentMontList;
            model.TotalMP = hblInCurrentMontList.Sum(x => x.MonthlyPension);
            model.TotalCMA = hblInCurrentMontList.Sum(x => x.CMA);
            model.TotalOrderly = hblInCurrentMontList.Sum(x => x.OrderlyAllowance);
            model.TotalDeduction = hblInCurrentMontList.Sum(x => x.Deduction);
            decimal TotalPaid = hblInCurrentMontList.Sum(x => x.Total);
            model.TotalPaid = TotalPaid;
            // Cheque Detail & Info
            var cheque = await _cheque.GetCheque(TotalPaid, month);
            if (cheque != null)
            {
                model.ChequeDate = cheque.Date;
                model.ChequeNumber = cheque.Number;
                model.ChequeAmount = cheque.Amount;
            }
            model.Session = new SessionViewModel()
            {
                DMStamp = _sessionHelper.GetDMStamp(),
                AMStamp = _sessionHelper.GetAMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp(),
                PDUId = _sessionHelper.GetUserPDUId()
            };
            DateOnly dateOnly = new(month.Year, month.Month, month.Day);
            model.HBLArrears = await _hBLArrears.GetArrearsAsync(dateOnly);
            return PartialView("_List", model);
        }

        public async Task<IActionResult> Consolidated()
        {
            ViewData["PensionerId"] = new SelectList(
                await _pensioner
                .GetOptions(_sessionHelper.GetUserPDUId(), false), "Value", "Text");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PPOPage()
        {
            ViewData["PensionerId"] = new SelectList(
                await _pensioner
                .GetOptions(_sessionHelper.GetUserPDUId(), false), "Value", "Text");
            return View("_PPOPage");
        }

        [HttpPost]
        public async Task<IActionResult> GetConsolidated(DateTime StartingMonth, DateTime EndingMonth)
        {
            if (EndingMonth < StartingMonth)
                return NotFound("Starting Date must be before the ending date ");

            DateTime StartingDate = new(StartingMonth.Year, StartingMonth.Month, 1);
            DateTime EndingDate = EndingMonth.AddMonths(1).AddDays(-1);

            var listHblPayments = await _hBLPayments
                .GetConsolidatedPensioner(StartingDate, EndingDate, _sessionHelper.GetUserPDUId());
            ConsolidatedSummaryModel model = new();
            if (listHblPayments != null)
            {
                var listPensionersOrderd = listHblPayments.GroupBy(x => x.Pensioner.Id)
                    .Select(g => g.First()).OrderBy(z => z.Pensioner.PPOSystem).ToList();
                var Months = listHblPayments.GroupBy(x => x.Month.Month)
                                        .Select(g => g.First()).OrderBy(z => z.Month).ToList();
                model.Pensioners = listPensionersOrderd;
                model.Months = Months;
            }
            model.HBLPayments = listHblPayments;
            var hblArrears = await _hBLArrears.GetArrearsByDates(StartingDate, EndingDate, _sessionHelper.GetUserPDUId());
            var listAllPensioners = await _hBLPayments.GetAllPensioners(StartingDate, EndingDate, _sessionHelper.GetUserPDUId());
            if (listAllPensioners != null)
            {
                model.AllPensioners = [.. listAllPensioners.OrderBy(x => x.PPOSystem)];
            }
            model.HBLArrears = hblArrears;
            model.Commutations = await _commutation.GetCommutationsByDates(StartingDate, EndingDate, _sessionHelper.GetUserPDUId());
            model.BankCharges = await _cashBook.GetBankChargesBetweenMonths(StartingDate, EndingDate, _sessionHelper.GetUserPDUId());
            model.Session = new SessionViewModel()
            {
                AMStamp = _sessionHelper.GetAMStamp(),
                DMStamp = _sessionHelper.GetDMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };
            return PartialView("_ConsolidatedSummary", model);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            var context = _context.HBLPayments;
            JsonResponseHelper jsonResponseHelper = new();
            if (context != null)
            {
                var item = await context.FirstOrDefaultAsync(x => x.Id == Id);
                if (item != null)
                {
                    try
                    {
                        context.Remove(item);
                        await _context.SaveChangesAsync();
                        jsonResponseHelper.RCode = 1;
                        jsonResponseHelper.RText = "";
                    }
                    catch (Exception exc)
                    {
                        jsonResponseHelper.RCode = 0;
                        jsonResponseHelper.RText = exc.Message.ToString();
                    }
                }
            }
            else
            {
                jsonResponseHelper.RCode = 0;
                jsonResponseHelper.RText = "ENtity error";
            }
            return Json(jsonResponseHelper);
        }

        [HttpGet]
        public async Task<IActionResult> GetByChequeId(int ChequeId)
        {
            var list = await _hBLPayments.GetByChequeId(ChequeId);
            if (list != null && list.Count == 0)
                return NotFound("No Data Found!");

            HBLPaymentViewModel? vm = new()
            {
                HBLPaymentsList = list,
                CurrentMonth = list.First().Month
            };
            var cheque = await _cheque.GetCheque(ChequeId);
            if (cheque != null)
            {
                vm.ChequeNumber = cheque.Number;
                vm.ChequeDate = cheque.Date;
                vm.ChequeAmount = cheque.Amount;
                vm.ChequePayee = cheque.Name;
            }
            vm.TotalMP = list.Sum(h => h.MonthlyPension);
            vm.TotalCMA = list.Sum(h => h.CMA);
            vm.TotalOrderly = list.Sum(h => h.OrderlyAllowance);
            vm.TotalDeduction = list.Sum(h => h.Deduction);
            vm.TotalPaid = list.Sum(x => x.Total);
            vm.Session = new SessionViewModel()
            {
                DMStamp = _sessionHelper.GetDMStamp(),
                AMStamp = _sessionHelper.GetAMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp(),
                PDUId = _sessionHelper.GetUserPDUId()
            };
            return PartialView("_List", vm);
        }

        [HttpPost]
        public async Task<JsonResponseHelper> SendSMSByChequeId(int ChequeId)
        {
            var list = await _hBLPayments.GetByChequeId(ChequeId);
            if (list != null && list.Count > 0)
            {
                var listMessage = new List<MessagesHistory>();
                string Claimant;
                string Mobile;
                string Account;
                string MessageToSent;
                string Amount;
                foreach (var item in list)
                {

                    if (item.Pensioner != null && item.Pensioner.IsServiceActive)
                    {
                        Mobile = item.Pensioner.Mobile;
                        Mobile = Mobile.Replace("-", "");
                        Mobile = Mobile.Replace("+", "");
                        Claimant = item.Pensioner.Claimant;
                        Account = item.Branch.Bank.ShortName + " (" + item.Branch.Code + "-" + item.AccountNumber + ")";
                        Amount = UserFormat.GetAmount(item.Total);
                        MessageToSent = "Dear " + Claimant + "!, your Pension Rs =" + Amount + "/- has been successfully sent to your bank " + Account + " Please kindly check it after one hour.";
                        var JsonResult = await SMSSender.SendSMS(Mobile, MessageToSent);
                        MessagesHistory messagesHistory = new()
                        {
                            MessageText = MessageToSent,
                            PensionerId = item.PensionerId,
                            ToNumber = Mobile,
                            JsonResponse = JsonResult
                        };
                        listMessage.Add(messagesHistory);
                    }
                    Mobile = string.Empty;
                    Claimant = string.Empty;
                    Account = string.Empty;
                    MessageToSent = string.Empty;
                    Amount = string.Empty;
                }
                await _context.MessagesHistories.AddRangeAsync(listMessage);
                await _context.SaveChangesAsync();
                return new JsonResponseHelper() { RCode = 1, RText = "Sent Successfully" };
            }
            else
            {
                return new JsonResponseHelper() { RCode = 0, RText = "No Pensioner In List" };
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChequeList(int ChequeId)
        {
            var list = await _hBLPayments.GetByChequeId(ChequeId);
            if (list != null && list.Count == 0)
                return NotFound("No Data Found!");

            HBLPaymentViewModel? vm = new()
            {
                HBLPaymentsList = list,
                CurrentMonth = list.First().Month
            };
            var cheque = await _cheque.GetCheque(ChequeId);
            if (cheque != null)
            {
                vm.ChequeNumber = cheque.Number;
                vm.ChequeDate = cheque.Date;
                vm.ChequeAmount = cheque.Amount;
                vm.ChequePayee = cheque.Name;
            }
            vm.TotalMP = list.Sum(h => h.MonthlyPension);
            vm.TotalCMA = list.Sum(h => h.CMA);
            vm.TotalOrderly = list.Sum(h => h.OrderlyAllowance);
            vm.TotalDeduction = list.Sum(h => h.Deduction);
            vm.TotalPaid = list.Sum(x => x.Total);
            vm.Session = new SessionViewModel()
            {
                AMStamp = _sessionHelper.GetAMStamp(),
                DMStamp = _sessionHelper.GetDMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };
            return PartialView("_ChequeList", vm);
        }

        [HttpPost]
        public async Task<IActionResult> PayByChequeNow(int ChequeId, int DemandId, int BankId)
        {
            if (ChequeId == 0 || DemandId == 0 || BankId == 0)
                return NotFound("Invalid Cheque or Demand or bank");
            /// validating cheque
            var cheque = await _cheque.GetCheque(ChequeId);
            if (cheque == null)
                return NotFound("Unable to find to cheque");
            var listPensionPayment = await _pensionPayment.GetPensionPayments(DemandId);
            if (listPensionPayment == null)
                return NotFound("Empty Demand");

            decimal DemandAmount = listPensionPayment
                .Where(x => x.Pensioner.Branch.BankId == BankId && x.CertificateVerified == true && x.PhysicallyVerified == true)
                .Sum(x => x.NetPension);
            if (DemandAmount > cheque.Amount)
                return NotFound("Bank Amount of the selected Demand  can not be greater then Cheque Amount <br> Demand : " + UserFormat.GetAmount(DemandAmount) + " ");

            var result = await _hBLPayments
                .PayDemandByCheque(ChequeId, DemandId, BankId);
            if (result != "Ok")
                return NotFound(result);

            return await GetByChequeId(ChequeId);
        }

        [HttpGet]
        public async Task<IActionResult> GetConsolidatedByPensionerId(int PensionerId)
        {
            HBLPaymentPensionerHistoryVM vm = new()
            {
                HBLPayments = await _hBLPayments.GetByPensionerId(PensionerId),
                Pensioner = await _pensioner.Get(PensionerId),
            };
            return PartialView("_PensionerHistory", vm);
        }
    }
}