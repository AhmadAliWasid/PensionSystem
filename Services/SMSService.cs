using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;

namespace PensionSystem.Services
{
    public class SMSService : ISMS
    {
        private readonly ApplicationDbContext _context;

        public SMSService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public Task<IQueryable<MessagesHistory>> Table => throw new NotImplementedException();

        IQueryable<MessagesHistory> ICrud<MessagesHistory>.Table => throw new NotImplementedException();

        public Task<bool> Delete(MessagesHistory entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<MessagesHistory>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public Task<List<MessagesHistory>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public MessagesHistory GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool IsSaved, string Message)> Insert(MessagesHistory entity)
        {
            throw new NotImplementedException();
        }

        public Task<(bool IsSaved, string Message)> Update(MessagesHistory entity)
        {
            throw new NotImplementedException();
        }

        Task<MessagesHistory?> ICrud<MessagesHistory>.GetById(object id)
        {
            throw new NotImplementedException();
        }
    }
}