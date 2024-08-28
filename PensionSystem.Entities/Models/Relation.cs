using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class Relation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// Self, Daughter,Widow,Son,Mother 
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// S/O
        /// </summary>
        public string? Short { get; set; }
    }
}