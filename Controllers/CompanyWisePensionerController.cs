using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;
using WebAPI.Helpers;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class CompanyWisePensionerController(ApplicationDbContext applicationDbContext,
        IHBLPayments hBLPayments, ICompany company, IHBLArrears hBLArrears, ICommutation commutation, SessionHelper sessionHelper) : Controller
    {
        private readonly ApplicationDbContext _context = applicationDbContext;
        private readonly IHBLPayments _hBLPayments = hBLPayments;
        private readonly ICompany _company = company;
        private readonly IHBLArrears _hBLArrears = hBLArrears;
        private readonly ICommutation _commutation = commutation;
        private readonly SessionHelper _sessionHelper = sessionHelper;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList(DateTime month)
        {
            DateOnly dateOnly = new(month.Year, month.Month, month.Day);
            CompanyWisePensionerViewModel model = new()
            {
                HBLPayments = await _hBLPayments.GetByMonth(month, _sessionHelper.GetUserPDUId()),
                HBLPaymentPensioners = await _hBLPayments.GetAllPensioners(month, month.AddMonths(1).AddDays(-1), _sessionHelper.GetUserPDUId()),
                Companies = await _company.GetCompanies(),
                HBLArrears = await _hBLArrears.GetArrearsByMonth(month, _sessionHelper.GetUserPDUId()),
                Commutations = await _commutation.GetCommutationsByDates(month, month.AddMonths(1).AddDays(-1), _sessionHelper.GetUserPDUId()),
                Month = month,
                Session = new SessionViewModel()
                {
                    AMStamp = _sessionHelper.GetAMStamp(),
                    DMStamp = _sessionHelper.GetDMStamp(),
                    BaseStamp = _sessionHelper.GetBaseStamp()
                }
            };
            return PartialView("_List", model);
        }
    }
}