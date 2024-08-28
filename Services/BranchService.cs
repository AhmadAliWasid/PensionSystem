using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class BranchService : IBranch
    {
        private readonly ApplicationDbContext _context;

        public BranchService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<List<Branch>> GetAll()
        {
            var cContext = _context.Branches;
            return await cContext.Include(x => x.Bank).OrderBy(c => c.Bank.Name).ToListAsync();
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions()
        {
            var rContext = _context.Branches;
            var options = new List<SelectOptions>();

            if (rContext != null)
            {
                var relations = await rContext.Include(b => b.Bank).ToListAsync();
                options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
                if (relations != null)
                {
                    foreach (var item in relations)
                    {
                        if (item.Name != null)
                            options.Add(new SelectOptions { Value = item.Id, Text = item.Code + " - " + item.Bank.ShortName + " (" + item.Name + ")" });
                    }
                }
            }
            return options;
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions(int bankId)
        {
            var rContext = _context.Branches;
            var options = new List<SelectOptions>();

            if (rContext != null)
            {
                var relations = await rContext.Where(x => x.BankId == bankId).ToListAsync();
                options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
                if (relations != null)
                {
                    foreach (var item in relations)
                    {
                        if (item.Name != null)
                            options.Add(new SelectOptions { Value = item.Id, Text = item.Code + " - " + item.Name });
                    }
                }
            }
            return options;
        }

        public async Task<BranchVM> GetVM(int id)
        {
            var cContext = _context.Branches;
            BranchVM branchVM = new();
            if (cContext == null)
            {
                return new BranchVM();
            }
            var branch = await cContext.FindAsync(id);
            if (branch == null)
            {
                return new BranchVM();
            }
            branchVM.Id = branch.Id;
            branchVM.Name = branch.Name;
            branchVM.Code = branch.Code;
            branchVM.BankId = branch.BankId;
            return branchVM;
        }

        public async Task<(bool IsSaved, string Message)> Save(BranchVM branchVM)
        {
            var cContext = _context.Branches;
            if (cContext == null)
                return (false, "Failed");
            try
            {
                if (branchVM.Id == 0)
                {
                    await cContext.AddAsync(new Branch() { BankId = branchVM.BankId, Name = branchVM.Name, Code = branchVM.Code });
                }
                else
                {
                    var branch = await cContext.FindAsync(branchVM.Id);
                    if (branch == null)
                        return (false, "Not Found");
                    branch.Name = branchVM.Name;
                    branch.Code = branchVM.Code;
                    branch.BankId = branchVM.BankId;
                    cContext.Update(branch);
                }
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