using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class RelationService : IRelation
    {
        private readonly ApplicationDbContext _context;

        public RelationService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions()
        {
            var rContext = _context.Relations;
            var options = new List<SelectOptions>();

            if (rContext != null)
            {
                var relations = await rContext.ToListAsync();
                options.Add(new SelectOptions { Value = 0, Text = "Select Relation" });
                if (relations != null)
                {
                    foreach (var item in relations)
                    {
                        if (item.Name != null)
                            options.Add(new SelectOptions { Value = item.Id, Text = item.Name });
                    }
                }
            }
            return options;
        }
    }
}