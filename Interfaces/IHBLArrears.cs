using PensionSystem.Entities.Models;

namespace PensionSystem.Interfaces
{
    public interface IHBLArrears
    {
        public Task<List<HBLArrears>?> Get();

        public Task<List<HBLArrears>?> GetArrearsAsync(DateOnly dateOnly);

        /// <summary>
        /// Get All HBL Arrears which is paid by Cheque
        /// </summary>
        /// <param name="chequeId"></param>
        /// <returns></returns>
        public Task<List<HBLArrears>> GetArrearsByCheque(int chequeId);

        public Task<string> PayHBLArrearsInBulk(List<ArrearsPayment> payments, int ChequeId, DateTime Month);

        public Task<List<HBLArrears>?> GetArrears(DateTime Month);
    }
}