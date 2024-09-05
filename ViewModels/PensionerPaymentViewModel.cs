using PensionSystem.Helpers;
using PensionSystem.Entities.Models;

namespace PensionSystem.ViewModels
{
    public class PensionerPaymentViewModel
    {
        public IEnumerable<Pensioner>? ActivePensioner { get; set; }
        public IEnumerable<PensionerPayment>? PayablesThisMonth { get; set; }
        public IEnumerable<Cheque>? Cheques { get; set; }
        public IEnumerable<MonthlyPensionDemand>? Demands { get; set; }
        public List<PensionerPayment>? LastMonthList { get; set; }
        public List<BankDemandVM>? BankDemandVMs { get; set; }
        public int DemandId { get; set; }
        public int ChequeId { get; set; }
        public DateTime Month { get; set; }
        public int Verified { get; set; }
        public int NonVerified { get; set; }
        public decimal AmountVerified { get; set; }
        public decimal AmountNonVerified { get; set; }
        public SessionViewModel? Session { get; set; }
    }

    public class PensionerLedgerVM
    {
        public Pensioner? Pensioner { get; set; }
        public List<PensionPaymentHistoryVM>? PensionerPayments { get; set; }
        public SessionViewModel? Session { get; set; }
    }

    public class PensionPaymentHistoryVM
    {
        public int DemandNo { get; set; }
        public DateTime DemandDate { get; set; }
        public DateTime Month { get; set; }
        public string Description { get; set; } = "N/A";
        public decimal MP { get; set; }
        public decimal CMA { get; set; }
        public decimal Orderly { get; set; }
        public decimal Deduction { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
    }

    public class BankDemandVM
    {
        public string BankName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}