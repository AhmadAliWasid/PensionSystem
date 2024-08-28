using PensionSystem.Helpers;
using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.ViewModels
{
    public class HBLArrearsViewModel
    {
        public Cheque? Cheque { get; set; }
        public List<HBLArrears>? HBLArrears { get; set; }
        public List<ArrearsDemand>? ArrearsDemand { get; set; }
        public SessionViewModel? Session { get; set; }
    }

    public class HBLArrearsCreateVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cheque is Required!")]
        [Display(Name = "Cheque")]
        public int ChequeId { get; set; }

        public Cheque? Cheque { get; set; }

        [Required(ErrorMessage = "Pensioner is required!")]
        [Display(Name = "Pensioner")]
        public int PensionerId { get; set; }

        [Required(ErrorMessage = "Payment Month Required!")]
        [Column(TypeName = "Date")]
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

        [Required(ErrorMessage = "Total is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Deduction")]
        public decimal Deduction { get; set; } = 0;
    }
}