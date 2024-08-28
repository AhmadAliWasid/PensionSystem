using System.ComponentModel.DataAnnotations;

namespace PensionSystem.ViewModels
{
    public class BranchVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch Name is Required!")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Branch Code is Required!")]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Bank")]
        public int BankId { get; set; }
    }
}