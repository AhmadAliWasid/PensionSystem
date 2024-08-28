using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PensionSystem.Entities.Models;

namespace PensionSystem.DTOs
{
    public class CashBookDTO
    {

        public int Id { get; set; }

        public DateTime Month { get; set; }

        public decimal Amount { get; set; }

        public TransactionType TransactionType { get; set; }

        public string Particulars { get; set; } = string.Empty;
    }
    public class CreateCashBookDTO
    {
        public DateTime Month { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Particulars { get; set; } = string.Empty;
    }
    public class UpdateCashBookDTO
    {
        public int Id { get; set; }
        public DateTime Month { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Particulars { get; set; } = string.Empty;
    }
}
