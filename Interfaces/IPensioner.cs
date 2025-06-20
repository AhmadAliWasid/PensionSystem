using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Interfaces;

namespace PensionSystem.Interfaces
{
    public interface IPensioner : ICrud<Pensioner>
    {
        public Task<(bool IsSaved, string Message)> Add(Pensioner pensioner);

        /// <summary>
        /// Get all active pensioners only
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public Task<List<Pensioner>> Get(bool isActive = true);

        /// <summary>
        /// Get All Restored Pensioners
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Pensioner>?> GetRestored(int PDUId);

        /// <summary>
        /// Get all pensioner for SMS where i can easily activate and de-activate sms service
        /// </summary>
        /// <returns></returns>
        public Task<List<Pensioner>?> GetSMSList(int PUIId);

        /// <summary>
        /// Get Option List By PDU Id
        /// </summary>
        /// <param name="isActive"></param>
        /// <param name="PDUId"></param>
        /// <returns></returns>
        public Task<IEnumerable<PensionerOption>> GetOptions(int PDUId, bool isActive = true);

        /// <summary>
        /// Get Max Page Number By Company Id for Adding New Pensioner
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Task<int> GetMaxPageNumber(int companyId);

        public Task<List<Pensioner>?> GetActive(int PDUId);

        /// <summary>
        /// Get All Pensioners By Company Id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Task<List<Pensioner>?> GetActiveByCompany(int companyId, int PDUId);

        /// <summary>
        /// Get Pensioner by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<Pensioner?> Get(int Id);

        /// <summary>
        /// Get All Employees of the said PDU
        /// </summary>
        /// <param name="PDUId"></param>
        /// <returns></returns>
        public Task<List<Pensioner>?> GetAll(int PDUId);

        /// <summary>
        /// Get All In Active Employees of the said PDU
        /// </summary>
        /// <param name="PDUId"></param>
        /// <returns></returns>
        public Task<List<Pensioner>?> GetInActiveList(int PDUId);

        public Task<List<Pensioner>?> GetPhysicalVerification(int PDUId);

        /// <summary>
        /// For Claimant Area we will use this
        /// </summary>
        /// <param name="PPONumber"></param>
        /// <param name="CNIC"></param>
        /// <returns></returns>
        public Task<Pensioner?> GetByPPO(int PPONumber);

        /// <summary>
        /// Restore evertything by pensioner id
        /// </summary>
        /// <param name="PensionerId"></param>
        /// <returns></returns>
        public Task<bool> RestoreNow(int PensionerId);

        /// <summary>
        /// Update the Status of Pensioner that the SMS service will be provided to them or not
        /// </summary>
        /// <param name="PensionerId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public Task<bool> UpdateSMSServiceStatus(int PensionerId, bool Status);

        /// <summary>
        /// Upload From File with Database Validation
        /// </summary>
        /// <param name="pensionerUploadVMs"></param>
        /// <returns></returns>
        public Task<(bool IsSaved, string Message)> AddToDBFromFile(List<PensionerUploadVM> pensionerUploadVMs);
        public Task<bool> UpdateAccountNumber(UpdateBranchDTO updateBranchDTO);
    }
}