using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface ICheque : ICrud<Cheque>
    {
        public Task<Cheque> GetCheque(int chequeId);

        /// <summary>
        /// Get Monthly Payments Cheque
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public Task<Cheque?> GetCheque(decimal Amount, DateTime dateTime);

        /// <summary>
        /// Get the options of Cheques for option element in list
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ChequeOption>> GetOptions(int PDUId);

        /// <summary>
        /// Get the options of Cheques for option
        /// 1 for Monthly
        /// 2 for Arrears
        /// 3. Commutation
        /// 1002. Widow Grant
        /// 2002. Marriage Grant
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ChequeOption>> GetOptions(int catId, int PDUId);

        /// <summary>
        /// Get the options of Cheques for option
        /// 1 for Monthly
        /// 2 for Arrears
        /// 3. Commutation
        /// 1002. Widow Grant
        /// 2002. Marriage Grant
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ChequeOption>> GetOptionsUnpaid(int catId);

        /// <summary>
        /// Get the options of Cheques for option
        /// 1 for Monthly
        /// 2 for Arrears
        /// 3. Commutation
        /// 1002. Widow Grant
        /// 2002. Marriage Grant
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ChequeOption>> GetOptionsUnpaidTwoCats(int catId1, int catId2);

        /// <summary>
        /// Get the List of Last 1000 Cheques
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Cheque>> GetList(int PDUId);

        /// <summary>
        /// Mark the chequeId id
        /// </summary>
        /// <param name="chequeId"></param>
        /// <returns></returns>
        public Task<bool> MarkItPay(int chequeId);

        public Task<List<Cheque>?> GetCheque(DateTime Month,int PDUIId);
        /// <summary>
        /// Get Latest Cheque Number
        /// </summary>
        /// <param name="PDUId"></param>
        /// <returns></returns>
        public Task<int> GetChequeNumber(int PDUId);
    }
}