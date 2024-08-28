using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class ConversionCase
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Dispatch Number Required!")]
        public string DispatchNo { get; set; } = "N/A";

        [Required(ErrorMessage = "Last PPO Number is Required!")]
        public string PPONumber { get; set; } = "N/A";

        [Required(ErrorMessage = "Subject or Name")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Dispatch Date Is Required")]
        [DisplayName("Dispathed On")]
        [DataType(DataType.Date)]
        public DateTime DispatchDate { get; set; }

        [DisplayName("Approved")]
        public bool IsAproved { get; set; } = false;

        [DataType(DataType.Date)]
        public DateTime? ApprovalDate
        {
            get; set;
        }

        [DisplayName("Diary #")]
        public string DiaryNumber { get; set; } = "";

        [DisplayName("Diary Date")]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? DiaryDate { get; set; }
    }
}