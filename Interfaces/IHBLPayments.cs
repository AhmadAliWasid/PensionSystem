using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IHBLPayments
    {
        public Task<List<HBLPayment>?> GetHBLPayments(DateOnly dateOnly);

        public Task<List<HBLPayment>?> GetConsolidatedPensioner(DateTime startingDate, DateTime endingDate);

        /// <summary>
        /// Get all pensioners with selected date who are paid
        /// </summary>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <returns></returns>
        public Task<List<HBLPaymentPensioner>?> GetAllPensioners(DateTime startingDate, DateTime endingDate);

        /// <summary>
        /// Get All HBL List By Cheque Id
        /// </summary>
        /// <param name="chequeId"></param>
        /// <returns></returns>
        public Task<List<HBLPayment>?> GetByChequeId(int chequeId);

        public Task<string> PayDemandByCheque(int chequeId, int DemandId, int BankId);

        public Task<List<HBLPaymentHistoryPensionerVM>?> GetByPensionerId(int PensionerId);

        public Task<List<HBLPayment>?> GetByMonth(DateTime month);
    }
}