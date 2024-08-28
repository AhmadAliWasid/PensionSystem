using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IBank
    {
        /// <summary>
        /// For Saving Bank Entity
        /// </summary>
        /// <param name="bankVM"></param>
        /// <returns></returns>
        public Task<(bool IsSaved, string Message)> Save(BankVM bankVM);

        public Task<List<Bank>> GetAll();

        public Task<Bank?> GetById(int id);

        public Task<IEnumerable<SelectOptions>> GetOptions();

        public Task<bool> Delete(int id);
    }
}