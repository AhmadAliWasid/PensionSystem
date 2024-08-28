using PensionSystem.ViewModels;

namespace PensionSystem.Interfaces
{
    public interface IChequeCategory
    {
        /// <summary>
        /// Get the options of Cheques Categories for option element in list
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SelectOptions>> GetOptions();
    }
}