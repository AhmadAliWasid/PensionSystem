using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class Commutation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cheque is Required!")]
        [Display(Name = "Cheque")]
        public int ChequeId { get; set; }

        public Cheque? Cheque { get; set; }

        [Required(ErrorMessage = "Pensioner is Required!")]
        [Display(Name = "Pensioner")]
        public int PensionerId { get; set; }

        public Pensioner? Pensioner { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Month { get; set; }

        [Required(ErrorMessage = "Amount is Required!")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater then or equal to zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Modified Date")]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}