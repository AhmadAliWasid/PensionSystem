using PensionSystem.Helpers;
using PensionSystem.Entities.Models;

namespace PensionSystem.ViewModels
{
    public class RestorationPaymentVM
    {
        public RestorationDemand? RestorationDemand { get; set; }
        public IEnumerable<RestorationPayment>? RestorationPayments { get; set; }
        public SessionViewModel? Session { get; set; }
    }
}