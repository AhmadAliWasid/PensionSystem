using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class MonthlyPensionDemandService(ApplicationDbContext applicationDbContext) : IMonthlyPensionDemand
    {
        private readonly ApplicationDbContext _context = applicationDbContext;

        public IQueryable<MonthlyPensionDemand> Table => _context.Set<MonthlyPensionDemand>();

        public Task<bool> Delete(MonthlyPensionDemand entity)
        {
            throw new NotImplementedException();
        }

        public async Task<MonthlyPensionDemand?> Get(int id)
        {
            return await Table.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<MonthlyPensionDemand>?> GetAll(int PDUId)
        {
            var cContext = _context.MonthlyPensionDemands;
            if (cContext == null)
                return null;
            var demand = await cContext
                .Where(p => p.PDUId == PDUId)
                .OrderByDescending(x => x.Date).Take(50)
                .ToListAsync();
            return demand;
        }

        public Task<List<MonthlyPensionDemand>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public Task<List<MonthlyPensionDemand>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public async Task<MonthlyPensionDemand?> GetById(object id)
        {
            return await Table.FirstOrDefaultAsync(x => x.Id == (int)id);
        }

        public async Task<List<SelectOptions>> GetOptions(DateTime dateTime)
        {
            var _MPD = _context.MonthlyPensionDemands;
            var options = new List<SelectOptions>();
            if (_MPD != null)
            {
                var cheques = await _MPD.Where(x => x.Date.Month == dateTime.Month && x.Date.Year == dateTime.Year).OrderBy(x => x.Number).ToListAsync();

                if (cheques != null)
                {
                    foreach (var item in cheques)
                    {
                        options.Add(new SelectOptions { Value = item.Id, Text = item.Number.ToString() + " - " + item.Date.ToString("dd-MM-yyyy") });
                    }
                }
            }
            return options;
        }

        public async Task<List<SelectOptions>> GetOptions(int PDUId)
        {
            var _MPD = _context.MonthlyPensionDemands;
            var options = new List<SelectOptions>();
            if (_MPD != null)
            {
                var cheques = await _MPD.Where(x => x.PDUId == PDUId)
                    .OrderByDescending(x => x.Date).ToListAsync();

                if (cheques != null)
                {
                    foreach (var item in cheques)
                    {
                        options.Add(new SelectOptions { Value = item.Id, Text = item.Number.ToString() + " - " + item.Date.ToString("dd-MM-yyyy") });
                    }
                }
            }
            return options;
        }

        public async Task<List<SelectOptions>> GetOptions(DateTime dateTime, int PDUIId)
        {
            var _MPD = _context.MonthlyPensionDemands;
            var options = new List<SelectOptions>();
            if (_MPD != null)
            {
                var cheques = await _MPD.
                    Where(x => x.Date.Month == dateTime.Month && x.Date.Year == dateTime.Year && x.PDUId == PDUIId)
                    .OrderBy(x => x.Number)
                    .ToListAsync();

                if (cheques != null)
                {
                    foreach (var item in cheques)
                    {
                        options.Add(new SelectOptions { Value = item.Id, Text = item.Number.ToString() + " - " + item.Date.ToString("dd-MM-yyyy") });
                    }
                }
            }
            return options;
        }

        public async Task<(bool IsSaved, string Message)> Insert(MonthlyPensionDemand entity)
        {
            try
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message);
            }
            
        }

        public Task<(bool IsSaved, string Message)> Update(MonthlyPensionDemand entity)
        {
            throw new NotImplementedException();
        }
    }
}