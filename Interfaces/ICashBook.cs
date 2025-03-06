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
    }
}