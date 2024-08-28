using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;

namespace PensionSystem.Services
{
    public class HBLArrearsService : IHBLArrears
    {
        private readonly ApplicationDbContext _context;
        private readonly ICheque _cheque;

        public HBLArrearsService(ApplicationDbContext applicationDbContext, ICheque cheque)
        {
            _context = applicationDbContext;
            _cheque = cheque;
        }

        public async Task<List<HBLArrears>?> Get()
        {
            var cContext = _context.HBLArrears;
            if (cContext == null)
                return null;
            return await cContext.Include(c => c.Cheque).Include(p => p.Pensioner).ToListAsync();
        }

        public async Task<List<HBLArrears>?> GetArrears(DateTime Month)
        {
            var cContext = _context.HBLArrears;
            if (cContext == null)
                return null;
            return await cContext
                .Include(p => p.Pensioner)
                .Include(r => r.Pensioner.Relation)
                .Include(b => b.Branch)
                .Include(ba => ba.Branch.Bank)
                .Where(x => x.Month.Month == Month.Month && x.Month.Year == Month.Year)
                .OrderBy(c => c.Pensioner.PPOSystem).ToListAsync();
        }

        public async Task<List<HBLArrears>?> GetArrearsAsync(DateOnly dateOnly)
        {
            try
            {
                var aContext = _context.HBLArrears;
                if (aContext == null)
                    return null;

                return await aContext.Include(c => c.Pensioner.Company)
                    .Where(x => x.Month.Month == dateOnly.Month && x.Month.Year == dateOnly.Year).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<HBLArrears>> GetArrearsByCheque(int chequeId)
        {
            var listCheques = _context.HBLArrears;
            var list = new List<HBLArrears>();
            if (listCheques != null)
            {
                list = await listCheques.Include(p => p.Pensioner)
                      .Include(b => b.Branch)
                      .Include(ba => ba.Branch.Bank)
                      .Where(x => x.ChequeId == chequeId)
                      .OrderBy(o => o.Pensioner.PPOSystem).ToListAsync();
            }
            return list;
        }

        public async Task<string> PayHBLArrearsInBulk(
            List<ArrearsPayment> payments, int ChequeId, DateTime PaymentMonth)
        {
            string response = string.Empty;
            if (payments.Count > 0)
            {
                var cContext = _context.HBLArrears;
                if (cContext != null)
                {
                    foreach (var item in payments)
                    {
                        if (item.Pensioner != null)
                        {
                            cContext.Add(new HBLArrears
                            {
                                ChequeId = ChequeId,
                                PensionerId = item.PensionerId,
                                FromMonth = item.FromMonth,
                                ToMonth = item.ToMonth,
                                Amount = item.Total,
                                Month = PaymentMonth,
                                Description = item.Description,
                                BranchId = item.Pensioner.BranchId,
                                AccountNumber = item.Pensioner.AccountNumber,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now
                            });
                        }
                    }
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception exc)
                    {
                        return ExceptionHelper.GetDetail(exc);
                    }
                }
            }
            return response;
        }
    }
}