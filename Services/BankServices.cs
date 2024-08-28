using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class BankServices : IBank
    {
        private readonly ApplicationDbContext _context;

        public BankServices(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<bool> Delete(int id)
        {
            var cContext = _context.Banks;
            if (cContext != null)
            {
                var bank = await cContext.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (bank != null)
                {
                    cContext.Remove(bank);
                    await _context.SaveChangesAsync();
                }
                return false;
            }
            return false;
        }

        public async Task<List<Bank>> GetAll()
        {
            return await _context.Banks.ToListAsync();
        }

        public async Task<Bank?> GetById(int id)
        {
            return await _context.Banks.FindAsync(id);
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions()
        {
            var rContext = _context.Banks;
            var options = new List<SelectOptions>();

            if (rContext != null)
            {
                var relations = await rContext.ToListAsync();
                options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
                if (relations != null)
                {
                    foreach (var item in relations)
                    {
                        if (item.Name != null)
                            options.Add(new SelectOptions { Value = item.Id, Text = item.ShortName });
                    }
                }
            }
            return options;
        }

        public async Task<(bool IsSaved, string Message)> Save(BankVM bankVM)
        {
            try
            {
                var bContext = _context.Banks;
                if (bankVM.Id == 0)
                {
                    Bank bank = new();
                    // add
                    bank.Name = bankVM.Name;
                    bank.ShortName = bankVM.ShortName;
                    bank.Limit = bankVM.Limit;
                    await bContext.AddAsync(bank);
                }
                else
                {
                    var bank = await bContext.Where(x => x.Id == bankVM.Id).SingleAsync();
                    bank.Name = bankVM.Name;
                    bank.ShortName = bankVM.ShortName;
                    bank.Limit = bankVM.Limit;
                    bContext.Update(bank);
                }
                await _context.SaveChangesAsync();
                return (true, "Ok");
            }
            catch (Exception exc)
            {
                return (false, ExceptionHelper.GetDetail(exc));
            }
        }
    }
}