using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class PensionerRelation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Relation Ship Title is Required!")]
        [StringLength(30, ErrorMessage = "Character Lenght Must not exceed 30 Characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Short Name is Required!")]
        [StringLength(20, ErrorMessage = "Character Length Must not exceed 20 Characters")]
        public string? ShortName { get; set; }
    }
}