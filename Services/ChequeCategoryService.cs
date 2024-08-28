using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class ChequeCategoryService : IChequeCategory
    {
        private readonly ApplicationDbContext _context;

        public ChequeCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions()
        {
            var cContext = _context.ChequeCategory;
            var options = new List<SelectOptions>();
            if (cContext == null)
                return options;
            var list = await cContext.ToListAsync();
            string Name;
            foreach (var item in list)
            {
                if (item.Name == null)
                    Name = "No Name";
                else
                    Name = item.Name;
                options.Add(new SelectOptions { Value = item.Id, Text = Name });
            }
            return options;
        }
    }
}