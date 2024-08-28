using System.ComponentModel.DataAnnotations;

namespace PensionSystem.ViewModels
{
    public class BankVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bank Name is Required!")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Short Name is Required!")]
        public string? ShortName { get; set; }

        [Required(ErrorMessage = "Numbers!")]
        public byte Limit { get; set; }
    }
}