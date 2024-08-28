namespace PensionSystem.Entities.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public int BankId { get; set; }

        public Bank Bank { get; set; }
    }
}