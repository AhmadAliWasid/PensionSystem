using Microsoft.AspNetCore.Mvc;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace PensionSystem.Areas.Claimant.Controllers
{
    [Area("Claimant")]
    public class PaymentHistoryController(IHBLPayments hBLPayments, IPensioner pensioner, IPensionPayment pensionPayment) : Controller
    {
        private readonly IHBLPayments _hBLPayments = hBLPayments;
        private readonly IPensioner _pensioner = pensioner;
        private readonly IPensionPayment _pensionPayment = pensionPayment;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetClaimantHistory(int PPONumber)
        {
            var p = await _pensioner.GetByPPO(PPONumber);
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