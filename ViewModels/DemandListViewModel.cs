using PensionSystem.Entities.Models;
using WebAPI.Helpers;

namespace PensionSystem.ViewModels
{
    public class DemandListViewModel
    {
        public DateOnly Month { get; set; }
        public List<ArrearsPayment>? ArrearsPayments { get; set; }
        public List<BankDemandVM>? bankDemandVMs { get; set; }
        public SessionViewModel? Session { get; set; }
    }
}