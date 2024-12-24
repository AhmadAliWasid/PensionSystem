using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class CompanyService : ICompany
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext applicationDbContext) => _context = applicationDbContext;

        public async Task<List<Company>?> GetCompanies()
        {
            var companies = _context.Company;
            return companies != null ? await companies.OrderBy(x => x.Order).ToListAsync() : null;
        }



        public async Task<List<SelectOptions>> GetOptions()
        {
            var cContext = _context.Company;
            var options = new List<SelectOptions>
            {
                new SelectOptions { Text = "Select a Company", Value = 0 }
            };
            if (cContext != null)
            {
                var cCompanies = await cContext.ToListAsync();
                foreach (var item in cCompanies)
                {
                    if (item.ShortName != null)
                        options.Add(new SelectOptions { Value = item.Id, Text = item.ShortName });
                }
            }
            return options;
        }
    }
}