using PensionSystem.Entities.Models;
using WebAPI.Helpers;

namespace PensionSystem.ViewModels
{
    public class RestorationPaymentVM
    {
        public RestorationDemand? RestorationDemand { get; set; }
        public IEnumerable<RestorationPayment>? RestorationPayments { get; set; }
        public SessionViewModel? Session { get; set; }
    }
}