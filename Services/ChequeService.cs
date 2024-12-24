using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using System.Linq.Expressions;

namespace PensionSystem.Services
{
    public class ChequeService(ApplicationDbContext applicationDbContext) : ICheque
    {
        private readonly ApplicationDbContext _context = applicationDbContext;

        public IQueryable<Cheque> Table => _context.Cheque;

        public Task<bool> Delete(Cheque entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Cheque>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            var walks = Table.Include(p => p.PDU).Include(c => c.ChequeCategory);
            var validPropertyNames = typeof(Cheque).GetProperties().Select(p => p.Name);
            // Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                // Check if filterOn is a valid property/column name
                if (validPropertyNames.Contains(filterOn, StringComparer.OrdinalIgnoreCase))
                {
                    var paramExpr = Expression.Parameter(typeof(Cheque), "x");
                    var propExpr = Expression.Property(paramExpr, filterOn);
                    var filterValueExpr = Expression.Constant(Convert.ChangeType(filterQuery, propExpr.Type)); // Convert filterQuery to the property type
                    var equalsExpr = Expression.Equal(propExpr, filterValueExpr);
                    var lambdaExpr = Expression.Lambda<Func<Cheque, bool>>(equalsExpr, paramExpr);
                    walks = (IIncludableQueryable<Cheque, ChequeCategory?>)walks.Where(lambdaExpr);
                }
                else
                {
                    // Handle invalid filterOn parameter here (e.g., throw an exception or log an error)
                    throw new ArgumentException("Invalid filterOn parameter");
                }
            }
            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                // Check if sortBy is a valid property/column name

            }
            // Pagination
            int skipResult = (pageNumber - 1) * pageSize;
            return await walks.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public Task<List<Cheque>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public async Task<Cheque?> GetById(object id)
        {
            return await Table
                .Include(c => c.ChequeCategory)
                .FirstOrDefaultAsync(x => x.Id == (int)id);
        }

        public async Task<Cheque?> GetCheque(decimal Amount, DateTime dateTime)
        {
            var chequesList = _context.Cheque;
            if (chequesList == null)
                return null;

            return await chequesList
                .Where(x => x.Amount == Amount && x.Date.Month == dateTime.Date.Month && x.Date.Year == dateTime.Date.Year && x.ChequeCategoryId == 1).FirstOrDefaultAsync();
        }

        public async Task<Cheque> GetCheque(int chequeId)
        {
            var cCheque = _context.Cheque;
            var cheque = new Cheque();
            if (cCheque != null)
            {
                return await cCheque.Where(x => x.Id == chequeId).FirstAsync();
            }
            return cheque;
        }
        public async Task<List<Cheque>?> GetCheque(DateTime Month, int PDUIId)
        {
            var cContext = _context.Cheque;
            if (cContext == null)
                return null;
            return await cContext
                .Where(x => x.Date.Month == Month.Month && x.Date.Year == Month.Year && x.PDUId == PDUIId)
                .OrderByDescending(c => c.Date)
                .ThenByDescending(c => c.Number)
                .ToListAsync();
        }

        public async Task<int> GetChequeNumber(int PDUId)
        {
            var r = await Table
                .Where(x => x.PDUId == PDUId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            if (r == null) return 0;
            int lastNumber = r.Number + 1;
            return lastNumber;
        }

        public async Task<IEnumerable<Cheque>> GetList(int PDUId)
        {
            var cContext = _context.Cheque;
            if (cContext == null)
                return new List<Cheque>();
            return await cContext
                .Include(x => x.ChequeCategory)
                .Where(p => p.PDUId == PDUId)
                .OrderByDescending(c => c.Date)
                .ThenByDescending(n => n.Number).ToListAsync();
        }

        public async Task<IEnumerable<ChequeOption>> GetOptions(int catId, int PDUId)
        {
            var pCheques = _context.Cheque;
            var options = new List<ChequeOption>
            {
                new() { Text = "", Value = 0 }
            };
            if (pCheques != null)
            {
                var cheques = await pCheques.
                    Where(c => c.ChequeCategoryId == catId && c.PDUId == PDUId)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(n => n.Number)
                    .ToListAsync();

                if (cheques != null)
                {
                    options.Add(new ChequeOption { Value = 0, Text = "Select Option" });
                    foreach (var item in cheques)
                    {
                        options.Add(new ChequeOption { Value = item.Id, Text = item.Number + " - " + UserFormat.GetAmount(item.Amount) });
                    }
                }
            }
            return options;
        }

        public async Task<IEnumerable<ChequeOption>> GetOptions(int PDUId)
        {
            var pCheques = _context.Cheque;
            var options = new List<ChequeOption>
            {
                new() { Text = "", Value = 0 }
            };
            if (pCheques != null)
            {
                var cheques = await pCheques
                    .Where(x => x.PDUId == PDUId)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(c => c.Number).ToListAsync();

                if (cheques != null)
                {
                    options.Add(new ChequeOption { Text = "Select Option", Value = 0 });
                    foreach (var item in cheques)
                    {
                        options.Add(new ChequeOption { Value = item.Id, Text = item.Number + " - " + UserFormat.GetAmount(item.Amount) });
                    }
                }
            }
            return options;
        }

        public async Task<IEnumerable<ChequeOption>> GetOptionsUnpaid(int catId, int PDUId)
        {
            var pCheques = _context.Cheque;
            var options = new List<ChequeOption>
            {
                new() { Text = "", Value = 0 }
            };
            if (pCheques != null)
            {
                var cheques = await pCheques
                    .Where(c => c.ChequeCategoryId == catId && c.IsLocked == false && c.PDUId == PDUId)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(c => c.Number)
                    .ToListAsync();
                if (cheques != null)
                {
                    options.Add(new ChequeOption { Value = 0, Text = "Select Option" });
                    foreach (var item in cheques)
                    {
                        options.Add(new ChequeOption { Value = item.Id, Text = item.Number + " - " + UserFormat.GetAmount(item.Amount) });
                    }
                }
            }
            return options;
        }

        public async Task<IEnumerable<ChequeOption>> GetOptionsUnpaidTwoCats(int catId1, int catId2, int PDUId)
        {
            var pCheques = _context.Cheque;
            var options = new List<ChequeOption>
            {
                new() { Text = "", Value = 0 }
            };
            if (pCheques != null)
            {
                var cheques = await pCheques
                    .Where(c => (c.ChequeCategoryId == catId1 || c.ChequeCategoryId == catId2)
                    && c.IsLocked == false && c.PDUId == PDUId)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(c => c.Number)
                    .ToListAsync();
                if (cheques != null)
                {
                    options.Add(new ChequeOption { Value = 0, Text = "Select Option" });
                    foreach (var item in cheques)
                    {
                        options.Add(new ChequeOption { Value = item.Id, Text = item.Number + " - " + UserFormat.GetAmount(item.Amount) });
                    }
                }
            }
            return options;
        }

        public async Task<(bool IsSaved, string Message)> Insert(Cheque entity)
        {
            try
            {
                await _context.Cheque.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "Ok");
            }
            catch (Exception exc)
            {
                return (false, exc.ToString());
            }
        }

        public async Task<bool> MarkItPay(int chequeId)
        {
            var cContext = _context.Cheque;
            var cheque = await cContext.FindAsync(chequeId);
            if (cheque == null)
                return false;
            try
            {
                cheque.IsLocked = true;
                cContext.Update(cheque);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<(bool IsSaved, string Message)> Update(Cheque entity)
        {
            try
            {
                _context.Cheque.Update(entity);
                await _context.SaveChangesAsync();
                return (true, "Ok");
            }
            catch (Exception exc)
            {
                return (false, exc.ToString());
            }
        }
    }
}