using Pension.Entities.Helpers;
using PensionSystem.Entities.Models;

namespace PensionSystem.Interfaces
{
    public interface IArrearsPayment : ICrud<ArrearsPayment>
    {
        /// <summary>
        /// Get Arrears Payment by Id Single record by Primary Key
        /// </summary>
        /// <param name="Id">id of the record</param>
        /// <returns></returns>
        public Task<ArrearsPayment>? Get(int Id);

        /// <summary>
        /// Get all payment by specified demand
        /// </summary>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public Task<List<ArrearsPayment>> GetByDemand(int demandId);

        /// <summary>
        /// Get all payment by specified demand
        /// </summary>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public Task<List<ArrearsPayment>> GetByDemandForPayment(int demandId, int BankId);

        public Task<(bool Valid, string Msg)> IsValid(int PensionerId, DateTime StartingDate, DateTime EndingDate);

        /// <summary>
        /// Clearing Demand by Demand ID
        /// </summary>
        /// <param name="DemandId"></param>
        /// <returns></returns>
        public Task<JsonResponseHelper> ClearDemand(int DemandId);
    }
}