using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Helpers;

namespace PensionSystem.ViewModels
{
    public class CashBookVM
    {
        public List<CashBook>? CashBooksEntries { get; set; }
        public DateTime Month { get; set; }
        public List<Cheque>? Cheques { get; set; }
        public List<HBLPayment>? MonthlPayment { get; set; }
        public List<HBLArrears>? HBLArrears { get; set; }
        public List<Commutation>? Commutations { get; set; }
        public List<CashBookEntryListVM>? CashBookEntryList { get; set; }
        public List<CashBookCompanyVM>? CashBookCompanyVMs { get; set; }
        public SessionViewModel? Session { get; set; }
    }

    public class CashBookEntryListVM
    {
        public int CashBookEntryId { get; set; }
        public int ChequeId { get; set; }
        public DateTime Month { get; set; }
    }

    public class CashBookCreateVM
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Month is Required!")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        public DateTime Month { get; set; }

        [Column(TypeName = "Decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount Must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Closing Balance is Required!")]
        [EnumDataType(typeof(TransactionType), ErrorMessage = "TransactionType must be 'debit' or 'credit'.")]
        public TransactionType TransactionType { get; set; }

        [Required(ErrorMessage = "Please Provide a detail of the transaction")]
        public string Particulars { get; set; }
        public int PDUId { get; set; }
    }

    public class CashBookCompanyVM
    {
        public int CompanyID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal SelfMP { get; set; }
        public decimal SelfCMA { get; set; }
        public decimal FamiliyMP { get; set; }
        public decimal FamilyCMA { get; set; }
        public decimal MP { get; set; }
        public decimal CMA { get; set; }
        public decimal Orderly { get; set; }
        public decimal Commutation { get; set; }
        public decimal BankCharges { get; set; }
        public decimal Total { get; set; }

        public decimal SelfArrearsMP { get; set; }
        public decimal FamilyArrearsMP { get; set; }
        public decimal SelfArrearsCMA { get; set; }
        public decimal SelfRecovery { get; set; }
        public decimal FamilyRecovery { get; set; }
    }
}