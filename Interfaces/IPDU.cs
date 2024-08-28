using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IPDU
    {
        /// <summary>
        /// Save or Update PDU
        /// </summary>
        /// <param name="pDU"></param>
        /// <returns></returns>
        public Task<(bool isSaved, string Message)> Save(PDU pDU);

        /// <summary>
        /// Getting PDU
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<PDU?> Get(int Id);

        /// <summary>
        /// All PDU is
        /// </summary>
        /// <returns></returns>
        public Task<List<PDU>> GetAll();

        public Task<IEnumerable<SelectOptions>> GetOptions();
    }
}