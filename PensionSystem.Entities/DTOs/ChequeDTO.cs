using PensionSystem.Entities.Models;

namespace PensionSystem.DTOs
{
    public class ChequeDTO
    {
        public int Id { get; set; }

        public int ChequeCategoryId { get; set; }

        public ChequeCategory ChequeCategory { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public int Number { get; set; }

        public bool IsLocked { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int PDUId { get; set; }
        public PDU PDU { get; set; }
    }
}