using PensionSystem.Entities.Models;

namespace PensionSystem.Interfaces
{
    public interface IRestorationPayment
    {
        public Task<IEnumerable<RestorationPayment>> GetRestorationPayments(int RestorationDemandId);
    }
}