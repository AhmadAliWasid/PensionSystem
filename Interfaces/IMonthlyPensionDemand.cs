using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IMonthlyPensionDemand : ICrud<MonthlyPensionDemand>
    {
        /// <summary>
        /// Get Monthly Pension Payment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<MonthlyPensionDemand?> Get(int id);

        /// <summary>
        /// Get Monthly Pension Payment
        /// </summary>
        /// <param name="PDUId"></param>
        /// <returns></returns>
        public Task<List<MonthlyPensionDemand>?> GetAll(int PDUId);

        /// <summary>
        /// Get the options of Cheques for option element in list
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Get the options of Cheques for option element in list
        /// </summary>
        /// <returns></returns>
        public Task<List<SelectOptions>> GetOptions(int PDUId);

        /// <summary>
        /// Get the options of Cheques for option element in list By Date
        /// </summary>
        /// <returns></returns>
        public Task<List<SelectOptions>> GetOptions(DateTime dateTime);

        /// <summary>
        /// Get the options of Cheques for option element in list By Date
        /// </summary>
        /// <returns></returns>
        public Task<List<SelectOptions>> GetOptions(DateTime dateTime, int PDUIId);
    }
}