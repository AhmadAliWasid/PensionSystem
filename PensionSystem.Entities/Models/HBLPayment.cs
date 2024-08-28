using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class HBLPayment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pensioner is Required!")]
        public int PensionerId { get; set; }

        [ForeignKey("PensionerId")]
        public Pensioner Pensioner { get; set; }

        [Required(ErrorMessage = "Account Number is Required!")]
        [StringLength(19, ErrorMessage = "Account Number Must be 16 Numbers")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "MonthlyPension is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyPension { get; set; }

        [Required(ErrorMessage = "CMA is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CMA { get; set; }

        [Required(ErrorMessage = "OrderlyAllowance is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderlyAllowance { get; set; }

        [Required(ErrorMessage = "Deduction is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Deduction { get; set; }

        [Required(ErrorMessage = "Total is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Month  is Required!")]
        [Column(TypeName = "Date")]
        public DateTime Month { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Modified Date is required!")]
        public DateTime ModifiedOn { get; set; }

        public int ChequeId { get; set; }
        public Cheque? Cheque { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}