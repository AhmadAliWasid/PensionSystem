using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class WWFBill
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pensioner Is Required!")]
        [Display(Name = "Pensioner")]
        [ForeignKey("Pensioner")]
        public int PensionerId { get; set; }

        public Pensioner? Pensioner { get; set; }

        [Required(ErrorMessage = "Sanction Number is Required!")]
        [StringLength(100, ErrorMessage = "Sanction Must not exceed 100 characters")]
        [Display(Name = "Sanction No.")]
        public string? SanctionNumber { get; set; }

        [Required(ErrorMessage = "Sanction Date is Required")]
        [Display(Name = "Sanction Date")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        public DateTime SanctionDate { get; set; }

        [Required(ErrorMessage = "Starting Month Required!")]
        [Display(Name = "From")]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime StartingMonth { get; set; }

        [Required(ErrorMessage = "Ending Month is Required")]
        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime EndingMonth { get; set; }

        [Required(ErrorMessage = "Please Specify Number of Month")]
        [Display(Name = "Months")]
        public byte NumberOfMonths { get; set; }

        [Required(ErrorMessage = "Monthly Pension  Rate is Required")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Total is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Created Date is Required!")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Modified Date is Required!")]
        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }
    }
}