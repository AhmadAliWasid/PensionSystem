using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IBranch
    {
        public Task<IEnumerable<SelectOptions>> GetOptions();

        public Task<IEnumerable<SelectOptions>> GetOptions(int bankId);

        /// <summary>
        /// Get Branch By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<BranchVM> GetVM(int id);

        public Task<(bool IsSaved, string Message)> Save(BranchVM bankVM);

        public Task<List<Branch>> GetAll();
    }
}