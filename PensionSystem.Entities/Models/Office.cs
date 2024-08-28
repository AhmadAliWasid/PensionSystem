using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class Office
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Account Title Required!")]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Official Name Required!")]
        public string? Official { get; set; }

        [Required(ErrorMessage = "Office Code is Required!")]
        [Display(Name = "Code")]
        public string? OfficeCode { get; set; }

        [Required(ErrorMessage = "Bank Name Required!")]
        [Display(Name = "Bank")]
        public string? Bank { get; set; }

        [Required(ErrorMessage = "Account Number Required!")]
        [Display(Name = "Account #")]
        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "Branch Name Required!")]
        [Display(Name = "Branch")]
        public string? Branch { get; set; }

        [Required(ErrorMessage = "Chargable To is Required!")]
        [Display(Name = "Chargable To")]
        public string? ChargableTo { get; set; }
    }
}