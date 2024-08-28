using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class Cheque
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cheque Category Is Required")]
        [Display(Name = "Category")]
        public int ChequeCategoryId { get; set; }

        [Display(Name = "Category")]
        public ChequeCategory? ChequeCategory { get; set; }

        [Required(ErrorMessage = "Name is required whose name cheque is issued")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Empty Cheque can not be issued")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required on which check is issued")]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Cheque Number is required!")]
        [Display(Name = "Cheque #")]
        public int Number { get; set; }

        [Display(Name = "Is Locked")]
        public bool IsLocked { get; set; } = false;

        [Display(Name = "Issued On")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Updated On")]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public int PDUId { get; set; }
        public PDU? PDU { get; set; }
    }
}