using PensionSystem.Helpers;
using PensionSystem.Entities.Models;

namespace PensionSystem.ViewModels
{
    public class CompanyWisePensionerViewModel
    {
        public IEnumerable<Company>? Companies { get; set; }
        public IEnumerable<HBLPayment>? HBLPayments { get; set; }
        public IEnumerable<HBLArrears>? HBLArrears { get; set; }
        public IEnumerable<Commutation>? Commutations { get; set; }
        public List<HBLPaymentPensioner>? HBLPaymentPensioners { get; set; }
        public DateTime Month { get; set; }
        public SessionViewModel? Session { get; set; }
    }
}