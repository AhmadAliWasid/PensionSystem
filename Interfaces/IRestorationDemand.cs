using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IRestorationDemand
    {
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
        public Task<IEnumerable<SelectOptions>> GetOptions();

        /// <summary>
        /// Get Unpaid Options of
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SelectOptions>> GetUnpaidOptions();

        /// <summary>
        /// Get options for select the incoming demand
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SelectOptions>> GetLatest();

        public Task<RestorationDemand?> GetRestorationDemand(int DemandId);
    }
}