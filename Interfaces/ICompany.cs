using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface ICompany
    {
        public Task<List<SelectOptions>> GetOptions();

        public Task<List<Company>?> GetCompanies();
    }
}