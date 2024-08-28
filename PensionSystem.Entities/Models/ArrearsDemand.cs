using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class ArrearsDemand
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Specify a demand Description")]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Number is Required!")]
        public int Number { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }
        public int PDUId { get; set; }
        public PDU? PDU { get; set; }
    }
}