﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class CompanyWisePensionerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHBLPayments _hBLPayments;
        private readonly ICompany _company;
        private readonly IHBLArrears _hBLArrears;
        private readonly ICommutation _commutation;
        private readonly SessionHelper _sessionHelper;

        public CompanyWisePensionerController(ApplicationDbContext applicationDbContext,
            IHBLPayments hBLPayments, ICompany company, IHBLArrears hBLArrears, ICommutation commutation, SessionHelper sessionHelper)
        {
            _context = applicationDbContext;
            _hBLPayments = hBLPayments;
            _company = company;
            _hBLArrears = hBLArrears;
            _commutation = commutation;
            _sessionHelper = sessionHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList(DateTime month)
        {
            DateOnly dateOnly = new(month.Year, month.Month, month.Day);
            CompanyWisePensionerViewModel model = new()
            {
                HBLPayments = await _hBLPayments.GetHBLPayments(dateOnly),
                HBLPaymentPensioners = await _hBLPayments.GetAllPensioners(month, month.AddMonths(1).AddDays(-1)),
                Companies = await _context.Company.ToListAsync(),
                HBLArrears = await _hBLArrears.GetArrearsAsync(dateOnly),
                Commutations = await _commutation.GetCommutationsByDates(month, month.AddMonths(1).AddDays(-1)),
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