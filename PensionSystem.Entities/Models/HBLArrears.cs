using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class HBLArrears
    {
        public int Id { get; set; }

        [ForeignKey("Cheque")]
        [Required(ErrorMessage = "Cheque is Required!")]
        [Display(Name = "Cheque")]
        public int ChequeId { get; set; }

        public Cheque? Cheque { get; set; }

        [ForeignKey("Pensioner")]
        [Required(ErrorMessage = "Pensioner is required!")]
        [Display(Name = "Pensioner")]
        public int PensionerId { get; set; }

        public Pensioner? Pensioner { get; set; }

        [Required(ErrorMessage = "Payment Month Required!")]
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Month")]
        public DateTime Month { get; set; }

        [Required(ErrorMessage = "Description is Required!")]
        [StringLength(30, ErrorMessage = "Descirption Must not exceed 30 characters")]
        public string? Description { get; set; }

        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "From Month is Required!")]
        [Display(Name = "From")]
        public DateTime FromMonth { get; set; }

        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "To Month is Required!")]
        [Display(Name = "To")]
        public DateTime ToMonth { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "Amount is Required!")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Account Number is Required!")]
        [MinLength(5, ErrorMessage = "Account Number Must Be Greater 19 Characters")]
        [Display(Name = "Account#")]
        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "Branch Id Required!")]
        public int BranchId { get; set; }

        public Branch? Branch { get; set; }

        [Required(ErrorMessage = "Total is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Deduction")]
        public decimal Deduction { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}