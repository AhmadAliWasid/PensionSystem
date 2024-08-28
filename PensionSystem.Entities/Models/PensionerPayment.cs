using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class PensionerPayment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pensioner is Required!")]
        [Display(Name = "Pensioner")]
        public int PensionerId { get; set; }

        [ForeignKey("PensionerId")]
        public Pensioner Pensioner { get; set; }

        /// <summary>
        /// Month is required field.
        /// </summary>
        [Required(ErrorMessage = "Month is Required!")]
        [Column(TypeName = "Date")]
        public DateTime Month { get; set; }

        /// <summary>
        /// Monthly Pension
        /// </summary>
        [Required(ErrorMessage = "MonthlyPension is Required!")]
        [Range(0, 500000, ErrorMessage = "Monthly Pension must be between 0 and 500000")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "MP")]
        public decimal MonthlyPension { get; set; }

        /// <summary>
        /// Cash Medical Allowence
        /// </summary>
        [Required(ErrorMessage = "CMA is Required!")]
        [Range(0, 500000, ErrorMessage = "Monthly Pension must be between 0 and 500000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CMA { get; set; }

        /// <summary>
        /// Orderely Allowence
        /// </summary>
        [Required(ErrorMessage = "MonthlyPension is Required!")]
        [Range(0, 500000, ErrorMessage = "Monthly Pension must be between 0 and 500000")]
        [Display(Name = "Orderly")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderelyAllowence { get; set; }

        /// <summary>
        /// Total Monthly Pension
        /// </summary>
        [Required(ErrorMessage = "MonthlyPension is Required!")]
        [Range(0, 500000, ErrorMessage = "Monthly Pension must be between 0 and 500000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        /// <summary>
        /// Deduction Pension
        /// </summary>
        [Required(ErrorMessage = "Deduction is Required!")]
        [Range(0, 500000, ErrorMessage = "Deduction must be between 0 and 500000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Deduction { get; set; }

        /// <summary>
        /// NetPension
        /// </summary>
        [Required(ErrorMessage = "NetPension is Required!")]
        [Range(0, 500000, ErrorMessage = "NetPension must be between 0 and 500000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetPension { get; set; }

        [Required(ErrorMessage = "Modified Date is Required!")]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Verification of Certificate if verified set true or else false
        /// </summary>
        [Required(ErrorMessage = "Certificate Verification is Required!")]
        public bool CertificateVerified { get; set; }

        /// <summary>
        /// Verification of Physicall if verified set true or else false
        /// </summary>
        [Required(ErrorMessage = "Physical Verification is Required!")]
        public bool PhysicallyVerified { get; set; }

        [Required(ErrorMessage = "Demand is required!")]
        public int MonthlyPensionDemandId { get; set; }

        public MonthlyPensionDemand MonthlyPensionDemand { get; set; }
    }
}