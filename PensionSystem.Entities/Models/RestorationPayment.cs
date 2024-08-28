using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class RestorationPayment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pensioner is Required!")]
        [Display(Name = "Pensioner")]
        public int PensionerId { get; set; }

        public Pensioner? Pensioner { get; set; }

        [Required(ErrorMessage = "From Month is Required!")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        [Display(Name = "From")]
        public DateTime FromMonth { get; set; }

        [Required(ErrorMessage = "To Month is Required!")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public DateTime ToMonth { get; set; }

        [Required(ErrorMessage = "No of Months Required")]
        [Display(Name = "No# Months")]
        public int NumberOfMonths { get; set; }

        [Required(ErrorMessage = "Monthly Pension is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "MP")]
        public decimal MP { get; set; }

        [Required(ErrorMessage = "CMA is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "CMA")]
        public decimal CMA { get; set; }

        [Required(ErrorMessage = "Orderly Allowance is Required!")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Orderly")]
        public decimal Orderly { get; set; }

        [Required(ErrorMessage = "Net Pension is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Net")]
        public decimal NetPension { get; set; }

        [Required(ErrorMessage = "Total is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Month is Required!")]
        [Column(TypeName = "Date")]
        [Display(Name = "Month")]
        public DateTime Month { get; set; }

        [Required(ErrorMessage = "Created On")]
        [Display(Name = "Created On")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Restoration Demand Id is Required")]
        [Display(Name = "Restoration Demand")]
        public int RestorationDemandId { get; set; }

        public RestorationDemand? RestorationDemand { get; set; }

        [Required(ErrorMessage = "Description is Required!")]
        [StringLength(30, ErrorMessage = "Descirption Must not exceed 30 characters")]
        public string? Description { get; set; }
    }
}