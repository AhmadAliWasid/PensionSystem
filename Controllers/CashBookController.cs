using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pension.Entities.Helpers;
using PensionSystem.DTOs;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class CashBookController : Controller
    {
        private readonly ICashBook _cashBook;
        private readonly IHBLPayments _hBLPayments;
        private readonly IHBLArrears _hBLArrears;
        private readonly ICheque _cheque;
        private readonly ICommutation _commutation;
        private readonly ICompany _company;
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClient _apiClient;

        public CashBookController(ICashBook cashBook, IHBLPayments hBLPayments, ICheque cheque,
            IHBLArrears hBLArrears, ICommutation commutation, ICompany company, SessionHelper sessionHelper, HttpClient apiClient)
        {
            _cashBook = cashBook;
            _hBLPayments = hBLPayments;
            _cheque = cheque;
            _hBLArrears = hBLArrears;
            _commutation = commutation;
            _company = company;
            _sessionHelper = sessionHelper;
            _apiClient = apiClient;
            _apiClient.BaseAddress = _sessionHelper.GetUri();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Crud()
        {
            return PartialView("_crud");
        }

        [HttpPost]
        public async Task<JsonResult> Save(CashBookCreateVM model)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                try
                {
                    CashBook cashBook = new()
                    {
                        Particulars = model.Particulars,
                        Month = model.Month,
                        Amount = model.Amount,
                        TransactionType = model.TransactionType,
                        PDUId = _sessionHelper.GetUserPDUId()
                    };
                    var (IsSaved, Message) = await _cashBook.Insert(cashBook);
                    if (IsSaved)
                    {
                        helper.RCode = 1;
                    }
                }
                catch (Exception exc)
                {
                    helper.RCode = 0;
                    helper.RText = exc.Message;
                }
            }
            else
            {
                helper.RCode = 0;
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var item in allErrors)
                {
                    helper.RText += item.ErrorMessage;
                }
            }
            return Json(helper);
        }

        [HttpGet]
        public async Task<IActionResult> Load(DateTime month)
        {
            DateOnly dateOnly = new(month.Year, month.Month, month.Day);
            var CashBookVM = new CashBookVM
            {
                Month = month,
                MonthlPayment = await _hBLPayments.GetByMonth(month),
                HBLArrears = await _hBLArrears.GetArrears(month),
                Cheques = await _cheque.GetCheque(month, _sessionHelper.GetUserPDUId()),
                Commutations = await _commutation.GetCommutations(dateOnly),
                CashBooksEntries = await _cashBook.GetByMonth(month, _sessionHelper.GetUserPDUId())
            };
            var list = new List<CashBookEntryListVM>();
            if (CashBookVM.CashBooksEntries != null && CashBookVM.CashBooksEntries.Count > 0)
            {
                foreach (var item in CashBookVM.CashBooksEntries)
                {
                    list.Add(new CashBookEntryListVM() { CashBookEntryId = item.Id, Month = item.Month });
                }
            }
            if (CashBookVM.Cheques != null && CashBookVM.Cheques.Count > 0)
            {
                foreach (var item in CashBookVM.Cheques)
                {
                    list.Add(new CashBookEntryListVM() { ChequeId = item.Id, Month = item.Date });
                }
            }
            var listCompanies = await _company.GetCompanies();
            var listCompaniesVM = new List<CashBookCompanyVM>();
            if (listCompanies != null)
            {
                foreach (var c in listCompanies)
                {
                    // monthly
                    decimal MPMonthly = CashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.MonthlyPension);
                    decimal CMAMonthly = CashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.CMA);
                    decimal OrderlyMonthly = CashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.OrderlyAllowance);
                    decimal selfMP = CashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self").Sum(y => y.MonthlyPension);
                    decimal selfCMA = CashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self").Sum(y => y.CMA);
                    decimal FamilyMP = CashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self").Sum(y => y.MonthlyPension);
                    decimal FamilayCMA = CashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self").Sum(y => y.CMA);
                    decimal Commutation = CashBookVM.Commutations.Where(x => x.Pensioner.CompanyId == c.Id).Sum(x => x.Amount);

                    listCompaniesVM.Add(new CashBookCompanyVM()
                    {
                        CompanyID = c.Id,
                        Name = c.ShortName,
                        MP = MPMonthly,
                        CMA = CMAMonthly,
                        Orderly = OrderlyMonthly,
                        SelfMP = selfMP,
                        SelfCMA = selfCMA,
                        FamilyCMA = FamilayCMA,
                        FamiliyMP = FamilyMP,
                        Commutation = Commutation
                    });
                }
            }
            CashBookVM.CashBookCompanyVMs = listCompaniesVM;
            CashBookVM.CashBookEntryList = list;
            CashBookVM.Session = new SessionViewModel()
            {
                AMStamp = _sessionHelper.GetAMStamp(),
                DMStamp = _sessionHelper.GetDMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };
            return PartialView("_List", CashBookVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetBreakUpSelfFamilyCompanyWise(DateTime Month)
        {
            DateOnly dateOnly = new(Month.Year, Month.Month, Month.Day);
            CashBookVM cashBookVM = new()
            {
                Month = Month,
                MonthlPayment = await _hBLPayments.GetByMonth(Month, _sessionHelper.GetUserPDUId()),
                HBLArrears = await _hBLArrears.GetArrearsByMonth(Month, _sessionHelper.GetUserPDUId()),
                Commutations = await _commutation.GetCommutationsByMonth(dateOnly, _sessionHelper.GetUserPDUId())
            };
            var listCompanies = await _company.GetCompanies();
            var listCompaniesVM = new List<CashBookCompanyVM>();
            if (listCompanies != null && cashBookVM.MonthlPayment != null && cashBookVM.Commutations != null)
            {
                foreach (var c in listCompanies)
                {
                    // monthly
                    decimal MPMonthly = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.MonthlyPension);
                    decimal CMAMonthly = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.CMA);
                    decimal OrderlyMonthly = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.OrderlyAllowance);
                    decimal selfMP = cashBookVM.MonthlPayment
                        .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self")
                        .Sum(y => y.MonthlyPension);
                    decimal selfCMA = cashBookVM.MonthlPayment
                        .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self")
                        .Sum(y => y.CMA);
                    decimal FamilyMP = cashBookVM.MonthlPayment
                        .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self")
                        .Sum(y => y.MonthlyPension);
                    decimal FamilayCMA = cashBookVM.MonthlPayment
                        .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self")
                        .Sum(y => y.CMA);
                    decimal Commutation = cashBookVM
                        .Commutations.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(x => x.Amount);
                    decimal SelfRecovery = cashBookVM.MonthlPayment
                        .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self")
                        .Sum(y => y.Deduction);
                    decimal FamilayRecovery = cashBookVM.MonthlPayment
                        .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self")
                        .Sum(y => y.Deduction);
                    // arrears
                    decimal SelfArrears = 0;
                    decimal FamilyArrears = 0;
                    if (cashBookVM.HBLArrears != null)
                    {
                        SelfArrears = cashBookVM.HBLArrears
                            .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self")
                            .Sum(y => y.Amount);
                        FamilyArrears = cashBookVM.HBLArrears
                           .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self")
                           .Sum(y => y.Amount);
                    }
                    listCompaniesVM.Add(new CashBookCompanyVM()
                    {
                        CompanyID = c.Id,
                        Name = c.ShortName,
                        MP = MPMonthly,
                        CMA = CMAMonthly,
                        Orderly = OrderlyMonthly,
                        SelfMP = selfMP,
                        SelfCMA = selfCMA,
                        FamilyCMA = FamilayCMA,
                        FamiliyMP = FamilyMP,
                        Commutation = Commutation,
                        SelfRecovery = SelfRecovery,
                        FamilyRecovery = FamilayRecovery,
                        SelfArrearsMP = SelfArrears,
                        FamilyArrearsMP = FamilyArrears,
                    });
                }
            }
            cashBookVM.CashBookCompanyVMs = listCompaniesVM;
            cashBookVM.Session = new SessionViewModel()
            {
                AMStamp = _sessionHelper.GetAMStamp(),
                DMStamp = _sessionHelper.GetDMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };

            return PartialView("_BreakUpSelfFamily", cashBookVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetBreakUpCompanyWise(DateTime Month)
        {
            DateOnly dateOnly = new(Month.Year, Month.Month, Month.Day);
            CashBookVM cashBookVM = new()
            {
                Month = Month,
                MonthlPayment = await _hBLPayments.GetByMonth(Month, _sessionHelper.GetUserPDUId()),
                HBLArrears = await _hBLArrears.GetArrearsByMonth(Month, _sessionHelper.GetUserPDUId()),
                Commutations = await _commutation.GetCommutationsByMonth(dateOnly, _sessionHelper.GetUserPDUId())
            };
            var listCompanies = await _company.GetCompanies();
            var listCompaniesVM = new List<CashBookCompanyVM>();
            if (listCompanies != null && cashBookVM.MonthlPayment != null && cashBookVM.Commutations != null && cashBookVM.HBLArrears != null)
            {
                foreach (var c in listCompanies)
                {
                    // monthly
                    decimal MPMonthly = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.MonthlyPension);
                    decimal CMAMonthly = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.CMA);
                    decimal OrderlyMonthly = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id)
                        .Sum(y => y.OrderlyAllowance);
                    decimal selfMP = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self").Sum(y => y.MonthlyPension);
                    decimal selfCMA = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self").Sum(y => y.CMA);
                    decimal FamilyMP = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self").Sum(y => y.MonthlyPension);
                    decimal FamilayCMA = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self").Sum(y => y.CMA);
                    decimal Commutation = cashBookVM.Commutations.Where(x => x.Pensioner.CompanyId == c.Id).Sum(x => x.Amount);
                    decimal SelfRecovery = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self").Sum(y => y.Deduction);
                    decimal FamilayRecovery = cashBookVM.MonthlPayment.Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self").Sum(y => y.Deduction);
                    // arrears
                    decimal SelfArrears = cashBookVM.HBLArrears
                        .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name == "Self")
                        .Sum(y => y.Amount);
                    decimal FamilyArrears = cashBookVM.HBLArrears
                       .Where(x => x.Pensioner.CompanyId == c.Id && x.Pensioner.Relation.Name != "Self")
                       .Sum(y => y.Amount);

                    listCompaniesVM.Add(new CashBookCompanyVM()
                    {
                        CompanyID = c.Id,
                        Name = c.ShortName,
                        MP = MPMonthly,
                        CMA = CMAMonthly,
                        Orderly = OrderlyMonthly,
                        SelfMP = selfMP,
                        SelfCMA = selfCMA,
                        FamilyCMA = FamilayCMA,
                        FamiliyMP = FamilyMP,
                        Commutation = Commutation,
                        SelfRecovery = SelfRecovery,
                        FamilyRecovery = FamilayRecovery,
                        SelfArrearsMP = SelfArrears,
                        FamilyArrearsMP = FamilyArrears,
                    });
                }
            }
            cashBookVM.CashBookCompanyVMs = listCompaniesVM;
            cashBookVM.Session = new SessionViewModel()
            {
                AMStamp = _sessionHelper.GetAMStamp(),
                DMStamp = _sessionHelper.GetDMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };

            return PartialView("_BreakUpCompanyWise", cashBookVM);
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            int pduId = _sessionHelper.GetUserPDUId(); // Replace with your PDUId value
            string filterOn = ""; // Add your filter values if needed
            string filterQuery = ""; // Add your filter values if needed
            string sortBy = ""; // Add your sort values if needed
            bool isAscending = true; // Add your sorting logic
            int pageNumber = 1;
            int pageSize = 1000;

            var request = $"/api/CashBook?PDUId={pduId}&filterOn={filterOn}&filterQuery={filterQuery}&sortBy={sortBy}&isAscending={isAscending}&pageNumber={pageNumber}&pageSize={pageSize}";
            HttpResponseMessage response = await _apiClient.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var list = new List<CashBookDTO>();
                // process the response content as needed
                // for example, you might deserialize the content
                var resultList = await response.Content.ReadFromJsonAsync<IEnumerable<CashBookDTO>>();
                if (resultList != null && resultList.Count() > 0)
                {
                    list.AddRange(resultList);
                }
                // then pass the cashbooklist to your view or use it as needed
                return PartialView("_ListCashBook", resultList);
            }
            else
            {
                // Handle the situation if the API call is unsuccessful
                return BadRequest();
            }

        }
    }
}