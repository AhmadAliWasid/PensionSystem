using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IRelation
    {
        public Task<IEnumerable<SelectOptions>> GetOptions();
    }
}