using PensionSystem.Entities.Models;

namespace PensionSystem.Interfaces
{
    public interface ICommutation : ICrud<Commutation>
    {
        public Task<List<Commutation>> GetCommutations();

        /// <summary>
        /// Get commutation
        /// </summary>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <returns></returns>
        public Task<List<Commutation>> GetCommutationsByDates(DateTime startingDate, DateTime endingDate);

        /// <summary>
        /// Get Commutations by Month
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        public Task<List<Commutation>> GetCommutations(DateOnly Month);
    }
}