using System.ComponentModel.DataAnnotations;

namespace PensionSystem.ViewModels
{
    public class PDUVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Short Name Required!")]
        [MaxLength(10)]
        [MinLength(3)]
        public string ShortName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name of PDU")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stamp First Line")]
        [Display(Name = "First Line")]
        public string AMStamp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stamp Second Line Required")]
        [Display(Name = "Second Line")]
        public string DMStamp { get; set; } = string.Empty;

        [Display(Name = "Third Line")]
        public string BaseStamp { get; set; } = string.Empty;
    }
}