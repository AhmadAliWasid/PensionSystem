using PensionSystem.Entities.Models;
using WebAPI.Helpers;

namespace PensionSystem.ViewModels
{
    public class HBLPaymentViewModel
    {
        public decimal TotalMP { get; set; }
        public decimal TotalCMA { get; set; }
        public decimal TotalOrderly { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime CurrentMonth { get; set; }
        public int ChequeNumber { get; set; }
        public decimal ChequeAmount { get; set; }
        public DateTime ChequeDate { get; set; }
        public string ChequePayee { get; set; } = string.Empty;
        public IEnumerable<HBLPayment>? HBLPaymentsList { get; set; }
        public List<HBLArrears>? HBLArrears { get; set; }
        public List<Pensioner>? Pensioners { get; set; }
        public  SessionViewModel? Session { get; set; }
    }

    public class HBLPaymentPensioner
    {
        public int PensionerId { get; set; }
        public int PPOSystem { get; set; }
        public string? Claimant { get; set; }
        public string? Designation { get; set; }

        public string? Name { get; set; }
        public int CompanyId { get; set; } = 0;
        public string CompanyName { get; set; } = " ";
        public int CompanyNumber { get; set; } = 0;
        public string ShortRel { get; set; } = string.Empty;
        public decimal MP { get; set; }
        public decimal CMA { get; set; }
        public decimal Orderly { get; set; }
        public decimal OrderlyAllowance { get; internal set; }
        public decimal Deduction { get; set; }
        public decimal Total { get; set; }
     
    }

    public class HBLPaymentPensionerHistoryVM
    {
        public List<HBLPaymentHistoryPensionerVM>? HBLPayments { get; set; }

        //public List<PensionPaymentHistoryVM>? PensionerPayments { get; set; }
        public List<ArrearsPayment>? ArrearsPayments { get; set; }

        public Pensioner? Pensioner { get; set; }
    }

    public class HBLPaymentHistoryPensionerVM
    {
        public int ChequeNo { get; set; }
        public DateTime Date { get; set; }
        public DateTime Month { get; set; }
        public string Description { get; set; } = "N/A";
        public string AccountNumber { get; set; } = "N/A";
        public decimal MP { get; set; }
        public decimal CMA { get; set; }
        public decimal Orderly { get; set; }
        public decimal Deduction { get; set; }
        public decimal Total { get; set; }
        public string Branch { get; set; } = string.Empty;
    }
}