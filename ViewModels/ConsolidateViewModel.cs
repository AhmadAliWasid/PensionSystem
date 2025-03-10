using PensionSystem.Helpers;
using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;
using PensionSystem.Entities.Helpers;

namespace PensionSystem.ViewModels
{
    public class ConsolidateViewModel
    {
    }

    public class ConsolidateSearchModel
    {
        [Required(ErrorMessage = "Starting Month is Required")]
        [DataType(DataType.Date)]
        public DateTime StartingMonth { get; set; }

        [Required(ErrorMessage = "Ending Month is Required")]
        [DataType(DataType.Date)]
        public DateTime EndingMonth { get; set; }
    }

    public class ConsolidatedSummaryModel
    {
        public List<HBLPayment>? HBLPayments { get; set; }
        public List<HBLPayment>? Pensioners { get; set; }
        public List<HBLPayment>? Months { get; set; }
        public List<Commutation>? Commutations { get; set; }
        public List<HBLArrears>? HBLArrears { get; set; }

        /// <summary>
        /// All Pensioners from Arrears and HBL Payments
        /// </summary>
        public List<HBLPaymentPensioner>? AllPensioners { get; set; }

        public SessionViewModel? Session { get; set; }
        /// <summary>
        /// Bank Charges of the given PDUI
        /// </summary>
        public List<BankCharge>? BankCharges { get; set; }
    }
}