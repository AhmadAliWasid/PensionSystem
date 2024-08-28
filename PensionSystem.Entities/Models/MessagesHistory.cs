namespace PensionSystem.Entities.Models
{
    public class MessagesHistory
    {
        public int Id { get; set; }
        public Pensioner? Pensioner { get; set; }
        public int PensionerId { get; set; }
        public string ToNumber { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public string JsonResponse { get; set; } = string.Empty;
    }
}