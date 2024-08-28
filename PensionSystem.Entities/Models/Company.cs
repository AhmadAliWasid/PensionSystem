using System.ComponentModel.DataAnnotations;

namespace PensionSystem.Entities.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is Required!")]
        [MaxLength(200, ErrorMessage = "Full Name Must Not Exceed 200 Characters")]
        [Display(Name = "Company")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Short Name is Required")]
        [MaxLength(10, ErrorMessage = "Company Short Name Must Not Exceed six Characters")]
        public string? ShortName { get; set; }

        [Required(ErrorMessage = "Order is Required!")]
        public int Order { get; set; }
    }
}