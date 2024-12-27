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
        public ChequeDTO ChequeDTOs { get; set; }
        public SessionViewModel SessionViewModel { get; set; }
    }
}