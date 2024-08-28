using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class CashBook
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Month is Required!")]
        [Column(TypeName = "Date")]
        public DateTime Month { get; set; }

        [Column(TypeName = "Decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount Must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Closing Balance is Required!")]
        [EnumDataType(typeof(TransactionType), ErrorMessage = "TransactionType must be 'debit' or 'credit'.")]
        public TransactionType TransactionType { get; set; }

        [Required(ErrorMessage = "Please Provide a detail of the transaction")]
        public string Particulars { get; set; }
    }

    public enum TransactionType
    {
        Debit,
        Credit
    }
}