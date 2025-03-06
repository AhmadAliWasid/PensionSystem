
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Interfaces;

namespace PensionSystem.Interfaces
{
    public interface IArrearsDemand : ICrud<ArrearsDemand>
    {
        /// <summary>
        /// Get List of Arreas Demand in Descending Order by Date
        /// </summary>
        /// <param name="PDUId"></param>
        /// <returns></returns>
        public Task<IEnumerable<ArrearsDemand>?> GetArrears(int PDUId);

        /// <summary>
        /// Get the month of current demand with full date
        /// </summary>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public DateOnly GetMonth(int demandId);

        /// <summary>
        /// Get options of select
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SelectOptions>> GetOptions(int PDUId);

        /// <summary>
        /// Get Unpaid Options of
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SelectOptions>> GetUnpaidOptions(int PDUId);

        /// <summary>
        /// Get options for select the incoming demand
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SelectOptions>> GetLatest();

        /// <summary>
        /// Return Arrears Demand by Demand Number or null if not exists
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public Task<ArrearsDemand?> GetByNumber(int number);

        /// <summary>
        /// Mark the demand id
        /// </summary>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public Task<bool> MarkItPay(int demandId);

        public Task<ArrearsDemand?> Get(int Id);
    }
}