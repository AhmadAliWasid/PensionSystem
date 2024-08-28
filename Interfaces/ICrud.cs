using PensionSystem.Entities.Models;

namespace PensionSystem.Interfaces
{
    public interface ICrud<T> where T : class
    {
        /// <summary>
        /// it will return list by filtering with sorting
        /// </summary>
        /// <param name="filterOn"></param>
        /// <param name="filterQuery"></param>
        /// <returns></returns>
        Task<List<T>> GetAll(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000);
        /// <summary>
        /// it will return list by filtering with sorting
        /// </summary>
        /// <param name="filterOn"></param>
        /// <param name="filterQuery"></param>
        /// <returns></returns>
        Task<List<T>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000);
        Task<T?> GetById(object id);

        Task<(bool IsSaved, string Message)> Insert(T entity);

        Task<(bool IsSaved, string Message)> Update(T entity);

        Task<bool> Delete(T entity);

        IQueryable<T> Table { get; }
    }
}