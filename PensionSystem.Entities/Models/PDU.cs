namespace PensionSystem.Entities.Models
{
    public class PDU
    {
        public int Id { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AMStamp { get; set; } = string.Empty;
        public string DMStamp { get; set; } = string.Empty;
        public string BaseStamp { get; set; } = string.Empty;
    }
}