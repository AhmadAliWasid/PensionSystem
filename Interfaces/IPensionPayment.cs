using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Interfaces;

namespace PensionSystem.Interfaces
{
    public interface IPensionPayment : ICrud<PensionerPayment>
    {
        /// <summary>
        /// Get The List of Pension Payment for Demand by Demand Id
        /// </summary>
        /// <param name="demandId"></param>
        /// <returns></returns>
        public Task<List<PensionerPayment>?> GetPensionPayments(int demandId);

        /// <summary>
        /// Generate List from Active Pensioners by Demand Id
        /// </summary>
        /// <param name="pensioners"></param>
        /// <param name="demandId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Task<string> GenerateDemandList(List<Pensioner> pensioners, int demandId, DateTime demandDate, int companyId);

        /// <summary>
        /// Get All List of Monthly Pensioners by Demand
        /// </summary>
        /// <param name="DemandId"></param>
        /// <returns></returns>
        public Task<List<PensionerPayment>?> GetByDemand(int DemandId);

        public Task<List<PensionPaymentHistoryVM>?> GetByPensionerId(int PensionerId);

        public Task<List<PensionerPayment>?> GetByMonth(DateTime dateTime);

        /// <summary>
        /// Verify Pensioner Physically
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> VerifyPhysically(int id);

        /// <summary>
        /// Verifiy Certifacte by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> VerifyCertificate(int id);
    }
}