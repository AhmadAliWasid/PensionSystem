using PensionSystem.Entities.Models;

namespace PensionSystem.Interfaces
{
    public interface IUserPDU
    {
        /// <summary>
        /// For Saving Bank Entity
        /// </summary>
        /// <param name="bankVM"></param>
        /// <returns></returns>
        public Task<(bool IsSaved, string Message)> Save(UserPDU userPDU);

        public Task<List<UserPDU>> GetAll();

        public Task<UserPDU?> GetByUserId(string UserId);
    }
}