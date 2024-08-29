using PensionSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Entities.DTOs
{
    public class ArreardDemandDTO
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public int Number { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }
        public int PDUId { get; set; }
    }
    public class CreateArreardDemandDTO
    {
        [Required(ErrorMessage = "Please Specify a demand Description")]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Number is Required!")]
        public int Number { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        public DateTime Date { get; set; }

        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }
        public int PDUId { get; set; }
    }
    public class UpdateArreardDemandDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Specify a demand Description")]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Number is Required!")]
        public int Number { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        public DateTime Date { get; set; }

        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }
        public int PDUId { get; set; }
    }
}
