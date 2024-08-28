using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class RestorationDemandService : IRestorationDemand
    {
        private readonly ApplicationDbContext _context;

        public RestorationDemandService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IEnumerable<SelectOptions>> GetLatest()
        {
            var aDContext = _context.RestorationDemands;
            var options = new List<SelectOptions>();
            if (aDContext != null)
            {
                var arreardDemand = await aDContext.Where(d => d.Date >= DateTime.Now.Date).OrderBy(x => x.Number).ToListAsync();
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
            var demandContext = _context.RestorationDemands;
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

        public async Task<IEnumerable<SelectOptions>> GetOptions()
        {
            var aDContext = _context.RestorationDemands;
            var options = new List<SelectOptions>();

            if (aDContext != null)
            {
                var arreardDemand = await aDContext.OrderBy(x => x.Number).ToListAsync();
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

        public async Task<RestorationDemand?> GetRestorationDemand(int DemandId)
        {
            return await _context.RestorationDemands.FindAsync(DemandId);
        }

        public async Task<IEnumerable<SelectOptions>> GetUnpaidOptions()
        {
            var aDContext = _context.RestorationDemands;
            var options = new List<SelectOptions>();

            if (aDContext != null)
            {
                var arreardDemand = await aDContext.Where(c => c.IsPaid == false).OrderBy(x => x.Number).ToListAsync();
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
    }
}