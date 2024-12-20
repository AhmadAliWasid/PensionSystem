using PensionSystem.Entities.Models;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace WebAPI.Interfaces
{
    public interface IWWFReimbursment : ICrud<WGReimbursment>
    {
        Task<(bool IsSaved, string Message)> LockIt(int id);

    }
}
