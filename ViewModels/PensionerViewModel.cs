using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.ViewModels
{
    public class PensionerViewModel
    {
        public IEnumerable<Pensioner>? Pensioners { get; set; }
        public List<Branch>? Branches { get; set; }
    }

    public class PhysicalVerificationVM
    {
        public List<Pensioner>? Pensioner { get; set; }
    }

    public class PensionerModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Page Number is required!")]
        [Display(Name = "Page")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "Pensioner Name is Required")]
        [MaxLength(50, ErrorMessage = "Pensioner Name must not exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Pensioner Name is wrong")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Claimant Name is Required")]
        [MaxLength(50, ErrorMessage = "Claimant Name must not exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Claimant Name is wrong")]
        public string? Claimant { get; set; } = "";

        [MaxLength(50, ErrorMessage = "Father Name must not exceed 50 characters")]
        [Display(Name = "F Name")]
        public string? FatherName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Spouse Name must not exceed 50 characters")]
        public string? Spouse { get; set; }

        [Required(ErrorMessage = "Designation is Required")]
        [MaxLength(50, ErrorMessage = "Designation must not exceed 50 characters")]
        [MinLength(2, ErrorMessage = "Designation is wrong")]
        public string? Designation { get; set; }

        /// <summary>
        /// Pension Payment Order
        /// </summary>
        [Required(ErrorMessage = "PPONumber is Required")]
        [MaxLength(50, ErrorMessage = "PPONumber must not exceed 50 characters")]
        [MinLength(2, ErrorMessage = "PPONumber is wrong")]
        [Display(Name = "PPO")]
        public string? PPONumber { get; set; }

        /// <summary>
        /// PPO On System
        /// </summary>
        [Display(Name = "PPO#")]
        public int PPOSystem { get; set; }

        /// <summary>
        /// Sanction Number
        /// </summary>
        [MaxLength(50, ErrorMessage = "Sanction must not exceed 50 characters")]
        [MinLength(2, ErrorMessage = "Sanction is wrong")]
        [Display(Name = "Sanction#")]
        public string? SanctionNumber { get; set; }

        /// <summary>
        /// Sanction Date
        /// </summary>
        [Display(Name = "Sanction Date")]
        public DateTime SanctionDate { get; set; }

        [Display(Name = "DOB")]
        public DateTime DOB { get; set; }

        [Display(Name = "Date Of Retirement")]
        [Column(TypeName = "Date")]
        public DateTime DateOfRetirement { get; set; }

        [Display(Name = "Company")]
        [Required(ErrorMessage = "You Must Select a Company if Not Exist add it from companies ")]
        public int CompanyId { get; set; }

        [MaxLength(15, ErrorMessage = "Contact Number Must not exceed 15 Characters")]
        [MinLength(15, ErrorMessage = "Contact Number Must be 15 Characters ")]
        public string Mobile { get; set; } = "";

        /// <summary>
        /// Pensioner CNIC
        /// </summary>
        [MaxLength(15, ErrorMessage = "CNIC Number Must not exceed 15 Characters")]
        [MinLength(15, ErrorMessage = "CNIC Number Must be  15 Characters ")]
        public string? CNIC { get; set; }

        /// <summary>
        /// Claimant CNIC
        /// </summary>
        [MaxLength(15, ErrorMessage = "CNIC Number Must not exceed 15 Characters")]
        [MinLength(15, ErrorMessage = "CNIC Number Must be  15 Characters ")]
        [Display(Name = "CNIC")]
        public string ClaimantCNIC { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Address Number Must not exceed 100 Characters")]
        [MinLength(5, ErrorMessage = "Address Number Must be  13 Characters ")]
        public string? Address { get; set; }

        /// <summary>
        /// for checking the pensioner is active or not
        /// </summary>
        [Display(Name = "Status")]
        public bool IsActiveClaimant { get; set; } = true;

        [Required(ErrorMessage = "Account Number is Required!")]
        [Display(Name = "Account#")]
        public string? AccountNumber { get; set; }

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
        public decimal OrderelyAllowence { get; set; }

        /// <summary>
        /// Total Monthly Pension
        /// </summary>
        [Required(ErrorMessage = "MonthlyPension is Required!")]
        [Range(0, 500000, ErrorMessage = "Monthly Pension must be between 0 and 500000")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Added Date is not specified")]
        public DateTime AddedDate { get; set; }

        [Required(ErrorMessage = "Modified Date is not specified")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(1000, ErrorMessage = "Remarks Must not exceed 1000 characters")]
        public string? Remarks { get; set; }

        /// <summary>
        /// Monthly Recovery is allowed to those who are overpaid
        /// </summary>
        [Required(ErrorMessage = "Monthly Recovery is Required!")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [Display(Name = "Recovery(33%)")]
        public decimal MonthlyRecovery { get; set; } = 0;

        [Required(ErrorMessage = "Gender is Required")]
        public char Gender { get; set; } = 'M';

        [Required(ErrorMessage = "Relation is Required!")]
        [Display(Name = "Relation")]
        public int RelationId { get; set; }

        [Required(ErrorMessage = "BPS is Required")]
        [Range(1, 22, ErrorMessage = "Scale or BPS Must be Between 01 to 22")]
        public int BPS { get; set; }

        public DateTime DateOfAppointment { get; set; }
        public decimal LastBasicPay { get; set; }

        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Please Specify a Bank")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Account Title is Required!")]
        [Display(Name = "Title")]
        public string AccountTitle { get; set; } = string.Empty;

        [Display(Name = "SMS Service")]
        public bool IsServiceActive { get; set; }

        [Required(ErrorMessage = "PDU Id is required!")]
        [Display(Name = "PDU")]
        public int PDUId { get; set; }

        [Required(ErrorMessage = "Commutation is required!")]
        public decimal Commutation { get; set; }

        [Required(ErrorMessage = "Ret. Office")]
        public string RetiringOffice { get; set; } = string.Empty;
    }

    public class PensionerOption
    {
        public string? Text { get; set; }
        public int Value { get; set; }
    }

    public class PensionerUploadVM
    {
        public int PDUId { get; set; }
        public int PPONumber { get; set; }

        [Required(ErrorMessage = "Pensioner Name is Required")]
        [MaxLength(50, ErrorMessage = "Pensioner Name must not exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Pensioner Name is wrong")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Designation is Required")]
        [MaxLength(50, ErrorMessage = "Designation must not exceed 50 characters")]
        [MinLength(2, ErrorMessage = "Designation is wrong")]
        public string? Designation { get; set; }

        public int BPS { get; set; }

        [Required(ErrorMessage = "Claimant Name is Required")]
        [MaxLength(50, ErrorMessage = "Claimant Name must not exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Claimant Name is wrong")]
        public string? Claimant { get; set; }

        [MaxLength(15, ErrorMessage = "CNIC Number Must not exceed 15 Characters")]
        [MinLength(15, ErrorMessage = "CNIC Number Must be  15 Characters ")]
        [Display(Name = "CNIC")]
        public string? ClaimantCNIC { get; set; }

        public string? CompanyShortName { get; set; }
        public string? BankShortName { get; set; }
        public string? BranchCode { get; set; }

        [Required(ErrorMessage = "Account Number is Required!")]
        [MaxLength(19, ErrorMessage = "Account Number Must not exceed 16 Characters")]
        [MinLength(19, ErrorMessage = "Account Number Must Be Greater 16 Characters")]
        [Display(Name = "Account#")]
        public string? AccountNumber { get; set; }

        public decimal MonthlyPension { get; set; }
        public decimal CMA { get; set; }
        public decimal Orderly { get; set; }
        public decimal Total { get; set; }
    }
}