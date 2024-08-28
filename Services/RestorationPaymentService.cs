using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;

namespace PensionSystem.Services
{
    public class RestorationPaymentService : IRestorationPayment
    {
        private readonly ApplicationDbContext _context;

        public RestorationPaymentService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<IEnumerable<RestorationPayment>> GetRestorationPayments(int RestorationDemandId)
        {
            var rContext = _context.RestorationPayments;
            IEnumerable<RestorationPayment> restorationPayments = new List<RestorationPayment>();
            if (rContext != null)
            {
                restorationPayments = await rContext.Include(p => p.Pensioner).Where(x => x.RestorationDemandId == RestorationDemandId).ToListAsync();
            }
            return restorationPayments;
        }
    }
}