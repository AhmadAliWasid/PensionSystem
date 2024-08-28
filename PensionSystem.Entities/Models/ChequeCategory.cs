using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class ChequeCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is Required!")]
        [StringLength(50, ErrorMessage = "Category Name must not ")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Specify a Date on which the cheque category is Created")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Specify a Modification Date for further references")]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}