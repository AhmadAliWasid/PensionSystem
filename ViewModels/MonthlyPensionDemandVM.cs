using PensionSystem.Entities.Models;

namespace PensionSystem.ViewModels
{
    public class MonthlyPensionDemandVM
    {
        public List<MonthlyPensionDemand>? monthlyPensionDemands { get; set; }
        public List<PensionerPayment>? PensionerPayments { get; set; }
    }

    public class MPDVMPrint
    {
        public List<PensionerPayment>? PensionerPayments { get; set; }
        public MonthlyPensionDemand? MonthlyPensionDemand { get; set; }
        public List<HBLArrears>? HBLArrears { get; set; }
        public List<HBLPayment>? HBLPayments { get; set; }
        public List<Commutation>? Commutations { get; set; }
    }
}