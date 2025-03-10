using PensionSystem.Entities.Helpers;
using PensionSystem.Entities.Models;
using WebAPI.Interfaces;

namespace PensionSystem.Interfaces
{
    public interface ICashBook : ICrud<CashBook>
    {
        /// <summary>
        /// Get List of Entries by Month
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        Task<List<CashBook>?> GetByMonth(DateTime Month,int PDUId);
        /// <summary>
        /// Getting the list of bank charges for given pdu id for consolidated etc.
        /// </summary>
        /// <param name="StartingMonth"></param>
        /// <param name="EndingMonth"></param>
        /// <param name="PDUId"></param>
        /// <returns></returns>
        Task<List<BankCharge>?> GetBankChargesBetweenMonths(DateTime StartingMonth, DateTime EndingMonth, int PDUId);
    }
}