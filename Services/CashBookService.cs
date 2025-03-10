using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using WebAPI.Interfaces;
using PensionSystem.Entities.Helpers;

namespace WebAPI.Services
{
    public class CashBookService(ApplicationDbContext context) : ICashBook
    {
        private readonly ApplicationDbContext _context = context;

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

        public async Task<List<BankCharge>?> GetBankChargesBetweenMonths(DateTime startingMonth, DateTime endingMonth, int pduId)
        {
            var result = await Table
                .Where(x => x.Month.Date >= startingMonth.Date && x.Month.Date <= endingMonth.Date &&
                            x.PDUId == pduId && x.TransactionType == TransactionType.Debit)
                .GroupBy(x => new { x.Month.Year, x.Month.Month })
                .Select(group => new BankCharge
                {
                    Month = new DateTime(group.Key.Year, group.Key.Month, 1),
                    Charges = group.Sum(item => item.Amount)
                })
                .ToListAsync();

            return result?.Any() == true ? result : null;
        }


        public async Task<CashBook?> GetById(object id)
        {
            return await _context.CashBook.Where(x => x.Id == (int)id).FirstOrDefaultAsync();
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

        public async Task<(bool IsSaved, string Message)> Update(CashBook entity)
        {
            try
            {
                var cContext = _context.CashBook;
                if (cContext == null)
                    return (false, "unable to fetch table");
                cContext.Update(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message);
            }
        }

    }
}