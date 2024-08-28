using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace PensionSystem.Entities.DTOs
{
    public class MPDemandDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int PDUId { get; set; }
        public PDU? PDU { get; set; }
    }
    public class CreateMPDemandDTO
    {
        [Required(ErrorMessage = "Please Specify a demand Description")]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Number is Required!")]
        public int Number { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int PDUId { get; set; }
    }
    public class UpdateMPDemandDTO : MPDemandDTO
    {

    }

}
