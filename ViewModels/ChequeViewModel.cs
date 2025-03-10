using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using PensionSystem.Helpers;

namespace WebAPI.ViewModels
{
    public class ChequeViewModel : Cheque
    {

    }

    public class ChequeOption
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    public class PVViewModel
    {
        public string Payee { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public List<(string desc,decimal amount)> Description { get; set; } = [];
        public int Number { get; set; }
        public SessionViewModel? SessionViewModel { get; set; }
    }
}