using PensionSystem.Entities.Models;

namespace PensionSystem.ViewModels
{
    public class ArrearsPaymentViewModel
    {
        public int ArrearsDemandId { get; set; }
        public List<ArrearsPayment>? ArrearsPayments { get; set; }
    }

    public class UploadPaymentVM
    {
        public List<Pensioner>? Pensioners { get; set; }
        public int ArrearsDemandId { get; set; }
    }

    public class UploadPaymentInsertModel
    {
        public int PensionerId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal MP { get; set; }
        public decimal CMA { get; set; }
        public decimal Orderly { get; set; }
        public decimal Net { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Months { get; set; }
        public decimal Deduction { get; set; }
        public decimal Total { get; set; }
    }
}