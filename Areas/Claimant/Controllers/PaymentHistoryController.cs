using Microsoft.AspNetCore.Mvc;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace PensionSystem.Areas.Claimant.Controllers
{
    [Area("Claimant")]
    public class PaymentHistoryController : Controller
    {
        private readonly IHBLPayments _hBLPayments;
        private readonly IPensioner _pensioner;
        private readonly IPensionPayment _pensionPayment;

        public PaymentHistoryController(IHBLPayments hBLPayments, IPensioner pensioner, IPensionPayment pensionPayment)
        {
            _hBLPayments = hBLPayments;
            _pensioner = pensioner;
            _pensionPayment = pensionPayment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetClaimantHistory(int PPONumber, string CNIC)
        {
            var p = await _pensioner.GetByPPOAndCNIC(PPONumber, CNIC);
            HBLPaymentPensionerHistoryVM vm = new()
            {
            };
            if (p != null)
            {
                vm.HBLPayments = await _hBLPayments.GetByPensionerId(p.Id);
                vm.Pensioner = await _pensioner.Get(p.Id);
            }
            return PartialView("_PensionerHistory", vm);
        }
    }
}