using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using System;

namespace WebAPI.Services
{
    public class ArrearsDemandService(ApplicationDbContext applicationDbContext) : IArrearsDemand
    {
        private readonly ApplicationDbContext _context = applicationDbContext;

        public IQueryable<ArrearsDemand> Table => _context.ArrearsDemands;

        public async Task<bool> Delete(ArrearsDemand entity)
        {
            try
            {
                _context.ArrearsDemands.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ArrearsDemand?> Get(int Id)
        {
            var context = _context.ArrearsDemands;
            if (context == null) { return null; }
            return await context.FindAsync(Id);
        }

        public async Task<List<ArrearsDemand>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table
               .ToListAsync();
        }

        public async Task<List<ArrearsDemand>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table
                .Where(x => x.PDUId == PDUId)
                .OrderByDescending(b => b.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArrearsDemand>?> GetArrears(int PDUId)
        {
            var aDContext = _context.ArrearsDemands;
            if (aDContext != null)
            {
                var arreardDemand = await aDContext
                    .Where(c => c.PDUId == PDUId)
                    .OrderByDescending(x => x.Date).ToListAsync();
                return arreardDemand;
            }
            return null;
        }

        public Task<ArrearsDemand?> GetById(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<ArrearsDemand?> GetByNumber(int number)
        {
            var ADContext = _context.ArrearsDemands;
            if (ADContext == null)
                return null;
            return await ADContext.Where(x => x.Number == number).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SelectOptions>> GetLatest()
        {
            var aDContext = _context.ArrearsDemands;
            var options = new List<SelectOptions>();

            if (aDContext != null)
            {
                var arreardDemand = await aDContext.Where(d => d.Date >= DateTime.Now.Date)
                    .OrderBy(x => x.Number).ToListAsync();
                if (arreardDemand != null)
                {
                    options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
                    foreach (var item in arreardDemand)
                    {
                        options.Add(new SelectOptions { Value = item.Id, Text = item.Number.ToString() });
                    }
                }
            }
            return options;
        }

        public DateOnly GetMonth(int demandId)
        {
            var demandContext = _context.ArrearsDemands;
            if (demandContext != null)
            {
                var demand = demandContext.Where(x => x.Id == demandId).First();
                return new DateOnly(demand.Date.Year, demand.Date.Month, demand.Date.Day);
            }
            else
            {
                return new DateOnly();
            }
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions(int PDUId)
        {
            var aDContext = _context.ArrearsDemands;
            var options = new List<SelectOptions>();

            if (aDContext != null)
            {
                var arreardDemand = await aDContext
                    .Where(x => x.PDUId == PDUId)
                    .OrderByDescending(x => x.Date).ToListAsync();
                if (arreardDemand != null)
                {
                    options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
                    foreach (var item in arreardDemand)
                    {
                        options.Add(new SelectOptions { Value = item.Id, Text = item.Number.ToString() + " - " + item.Description });
                    }
                }
            }
            return options;
        }

        public async Task<IEnumerable<SelectOptions>> GetUnpaidOptions(int PDUId)
        {
            var aDContext = _context.ArrearsDemands;
            var options = new List<SelectOptions>();

            if (aDContext != null)
            {
                var arreardDemand = await aDContext.Where(c => c.IsPaid == false && c.PDUId == PDUId)
                    .OrderByDescending(x => x.Date).ToListAsync();
                if (arreardDemand != null)
                {
                    options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
                    foreach (var item in arreardDemand)
                    {
                        options.Add(new SelectOptions { Value = item.Id, Text = item.Number.ToString() + " - " + item.Description });
                    }
                }
            }
            return options;
        }

        public async Task<(bool IsSaved, string Message)> Insert(ArrearsDemand entity)
        {
            try
            {
                await _context.ArrearsDemands.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message.ToString());
            }
        }

        public async Task<bool> MarkItPay(int demandId)
        {
            var cContext = _context.ArrearsDemands;
            if (cContext == null)
                return false;
            var entity = await cContext.FindAsync(demandId);
            if (entity == null)
                return false;
            entity.IsPaid = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool IsSaved, string Message)> Update(ArrearsDemand entity)
        {
            try
            {
                _context.ArrearsDemands.Update(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message.ToString());
            }
        }
    }
}