using PensionSystem.Entities.Models;

namespace PensionSystem.ViewModels
{
    public class ChequeViewModel : Cheque
    {
    }

    public class ChequeOption
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}