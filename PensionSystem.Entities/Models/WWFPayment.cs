using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class WWFPayment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pensioner is Required!")]
        [ForeignKey("Pensioner")]
        [Display(Name = "Pensioner")]
        public int PensionerId { get; set; }

        public Pensioner? Pensioner { get; set; }

        [ForeignKey("WWFDemand")]
        [Display(Name = "Demand#")]
        public int WWFDemandId { get; set; }

        public WWFDemand? WWFDemand { get; set; }

        [Required(ErrorMessage = "From Date is Required!")]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime From { get; set; }

        [Required(ErrorMessage = "To Date is Required!")]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime To { get; set; }

        [Required(ErrorMessage = "Number of Months Required!")]
        [Range(1, 48, ErrorMessage = "Arrears Must Not exceed two years or 24 Months")]
        public int Months { get; set; }

        [Required(ErrorMessage = "Rate is Required")]
        [Range(1, double.MaxValue, ErrorMessage = "Rate must be greater then 0")]
        [Column(TypeName = "Decimal(18,2)")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Amount is Required!")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater then 0")]
        [Column(TypeName = "Decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}