using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;

namespace PensionSystem.Services
{
    public class CashBookService : ICashBook
    {
        private readonly ApplicationDbContext _context;

        public CashBookService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<CashBook> Table => _context.Set<CashBook>();

        public Task<bool> Delete(CashBook entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<CashBook>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CashBook>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            var records = Table;
            return await records
                .Where(p => p.PDUId == PDUId)
                .OrderByDescending(x => x.Month)
                .ToListAsync();
        }

        public CashBook GetById(object id)
        {
            throw new NotImplementedException();
        }
        public async Task<List<CashBook>?> GetByMonth(DateTime Month, int PDUId)
        {
            var cContext = _context.CashBook;
            if (cContext == null)
            {
                return null;
            }
            return await cContext
                .Where(x => x.Month.Year == Month.Year && x.Month.Month == Month.Month && x.PDUId == PDUId)
                .OrderBy(x => x.Month)
                .ToListAsync();
        }

        public async Task<(bool IsSaved, string Message)> Insert(CashBook entity)
        {
            try
            {
                var cContext = _context.CashBook;
                if (cContext == null)
                    return (false, "Unable To Found DB");
                await cContext.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message);
            }
        }

        public Task<(bool IsSaved, string Message)> Update(CashBook entity)
        {
            throw new NotImplementedException();
        }

        Task<CashBook?> ICrud<CashBook>.GetById(object id)
        {
            throw new NotImplementedException();
        }
    }
}