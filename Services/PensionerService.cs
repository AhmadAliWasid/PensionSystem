using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using System.Linq.Expressions;
using PensionSystem.Entities.DTOs;

namespace WebAPI.Services
{
    public class PensionerService(ApplicationDbContext context) : IPensioner
    {
        private readonly ApplicationDbContext _context = context;
        public IQueryable<Pensioner> Table => _context.Set<Pensioner>();

        public async Task<(bool IsSaved, string Message)> Add(Pensioner pensioner)
        {
            try
            {
                _context.Add(pensioner);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message);
            }
        }

        public async Task<(bool IsSaved, string Message)> AddToDBFromFile(List<PensionerUploadVM> pensionerUploadVMs)
        {
            var pdus = await _context.PDUs.ToListAsync();
            var companies = await _context.Company.ToListAsync();
            var banks = await _context.Banks.ToListAsync();
            var branches = await _context.Branches.Include(x => x.Bank).ToListAsync();
            var relations = await _context.Relations.ToListAsync();

            if (pdus == null || companies == null || banks == null || branches == null || relations == null)
                return (false, "Required data not found in DB");

            try
            {
                var pContext = _context.Pensioner;
                if (pContext == null)
                    return (false, "Pension Table Does not Exist in DB");

                var pduId = pensionerUploadVMs.First().PDUId;
                var pInDB = await pContext.Where(x => x.PDUId == pduId).ToListAsync();
                var listPensioners = new List<Pensioner>();

                foreach (var item in pensionerUploadVMs)
                {
                    var currentPDU = pdus.FirstOrDefault(x => x.Id == item.PDUId);
                    if (currentPDU == null)
                        return (false, $"PDU not found for PPO #{item.PPONumber}");

                    var currentCompany = companies.FirstOrDefault(x => x.ShortName == item.CompanyShortName);
                    if (currentCompany == null)
                        return (false, $"Company not found for PPO #{item.PPONumber}");

                    var currentBank = banks.FirstOrDefault(x => x.ShortName == item.BankShortName);
                    if (currentBank == null)
                        return (false, $"Bank not found for PPO #{item.PPONumber}");

                    var currentBranch = branches.FirstOrDefault(x => x.Code.ToString() == item.BranchCode.ToString() && x.BankId == currentBank.Id);
                    if (currentBranch == null)
                        return (false, $"Branch not found for PPO #{item.PPONumber}");

                    bool isSelf = item.Claimant.ToUpper() == "SELF";
                    int pNumber = item.PPONumber;
                    if (pInDB.Any(x => x.PPOSystem == pNumber)) continue;

                    listPensioners.Add(new Pensioner
                    {
                        AccountNumber = item.AccountNumber,
                        AccountTitle = isSelf ? item.Name : item.Claimant,
                        Name = item.Name.Trim(),
                        Claimant = item.Claimant.Trim(),
                        AddedDate = DateTime.Now,
                        Address = "N/A",
                        BPS = item.BPS,
                        BranchId = currentBranch.Id,
                        DOB = DateTime.Now,
                        Designation = item.Designation.Trim(),
                        Gender = isSelf ? 'M' : 'F',
                        PDUId = item.PDUId,
                        PPONumber = item.PPONumber.ToString().Trim(),
                        PPOSystem = item.PPONumber,
                        ClaimantCNIC = item.ClaimantCNIC.Trim(),
                        Commutation = 0,
                        DateOfAppointment = DateTime.Now,
                        DateOfRetirement = DateTime.Now,
                        CompanyId = currentCompany.Id,
                        FatherName = "N/A",
                        IsActiveClaimant = true,
                        IsRestored = false,
                        CNIC = item.ClaimantCNIC.Trim(),
                        Mobile = "",
                        IsServiceActive = false,
                        LastBasicPay = 0,
                        MonthlyPension = item.MonthlyPension,
                        CMA = item.CMA,
                        OrderelyAllowence = item.Orderly,
                        Total = item.Total,
                        MonthlyRecovery = 0,
                        RelationId = isSelf ? 1 : 3,
                        SanctionNumber = "N/A",
                        SanctionDate = DateTime.Now,
                        Spouse = item.Name.Trim(),
                        RetiringOffice = "NA",
                        ModifiedDate = DateTime.Now,
                        PageNumber = 0,
                        Remarks = "New Added Using Bulk",
                        RestorationDate = DateTime.Now
                    });
                }

                await pContext.AddRangeAsync(listPensioners);
                await _context.SaveChangesAsync();
                return (true, "Saved Data Successfully!");
            }
            catch (Exception exc)
            {
                return (false, ExceptionHelper.GetDetail(exc));
            }
        }

        public Task<bool> Delete(Pensioner entity) => throw new NotImplementedException();

        public async Task<Pensioner?> Get(int Id) =>
            await _context.Pensioner
                .Include(c => c.Company)
                .Include(d => d.Branch)
                .Include(e => e.Branch.Bank)
                .Include(x => x.Relation)
                .FirstOrDefaultAsync(p => p.Id == Id);

        public async Task<List<Pensioner>> Get(bool isActive = true) =>
            await (_context.Pensioner?.ToListAsync() ?? Task.FromResult(new List<Pensioner>()));

        public async Task<List<Pensioner>?> GetActive(int PDUId) =>
            await _context.Pensioner
                .Include(c => c.Company)
                .Include(r => r.Relation)
                .Where(x => x.IsActiveClaimant && x.PDUId == PDUId)
                .OrderBy(y => y.PPOSystem).ToListAsync();

        public async Task<List<Pensioner>?> GetActiveByCompany(int companyId, int PDUId) =>
            await _context.Pensioner
                .Where(x => x.IsActiveClaimant && x.CompanyId == companyId && x.PDUId == PDUId)
                .OrderBy(y => y.PPOSystem).ToListAsync();

        public async Task<List<Pensioner>?> GetAll(int PDUId) =>
            await _context.Pensioner
                .Include(b => b.Relation)
                .Include(x => x.Branch)
                .Include(c => c.Branch.Bank)
                .Where(p => p.PDUId == PDUId)
                .OrderBy(d => d.PPOSystem).ToListAsync();

        public async Task<List<Pensioner>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            var query = Table;
            var validProps = typeof(Pensioner).GetProperties().Select(p => p.Name);

            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery) && validProps.Contains(filterOn, StringComparer.OrdinalIgnoreCase))
            {
                var param = Expression.Parameter(typeof(Pensioner), "x");
                var prop = Expression.Property(param, filterOn);
                var value = Expression.Constant(Convert.ChangeType(filterQuery, prop.Type));
                var equals = Expression.Equal(prop, value);
                var lambda = Expression.Lambda<Func<Pensioner, bool>>(equals, param);
                query = query.Where(lambda);
            }

            if (!string.IsNullOrWhiteSpace(sortBy) && validProps.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            {
                var param = Expression.Parameter(typeof(Pensioner), "x");
                var prop = Expression.Property(param, sortBy);
                var lambda = Expression.Lambda<Func<Pensioner, object>>(Expression.Convert(prop, typeof(object)), param);
                query = (IsAscending ?? true) ? query.OrderBy(lambda) : query.OrderByDescending(lambda);
            }

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public Task<List<Pensioner>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
            => throw new NotImplementedException();

        public async Task<Pensioner?> GetById(object id) =>
            await _context.Pensioner
                .Include(b => b.Branch)
                .Include(ba => ba.Branch.Bank)
                .Include(c => c.Company)
                .Include(r => r.Relation)
                .FirstOrDefaultAsync(x => x.Id == (int)id);

        public async Task<Pensioner?> GetByPPO(int PPONumber) =>
            await _context.Pensioner.FirstOrDefaultAsync(x => x.PPOSystem == PPONumber);

        public async Task<Pensioner?> GetByPPOAndCNIC(int PPONumber, string CNIC) =>
            await _context.Pensioner.FirstOrDefaultAsync(x => x.PPOSystem == PPONumber && x.ClaimantCNIC == CNIC);

        public async Task<List<Pensioner>?> GetInActiveList(int PDUId) =>
            await _context.Pensioner
                .Include(b => b.Relation)
                .Include(x => x.Branch)
                .Include(c => c.Branch.Bank)
                .Include(co => co.Company)
                .Where(p => p.PDUId == PDUId && !p.IsActiveClaimant)
                .OrderBy(d => d.PPOSystem).ToListAsync();

        public async Task<int> GetMaxPageNumber(int companyId) =>
            await _context.Pensioner.Where(x => x.CompanyId == companyId).MaxAsync(p => (int?)p.PageNumber) ?? 0;

        public async Task<IEnumerable<PensionerOption>> GetOptions(int PDUId, bool isActive = true)
        {
            var pContext = _context.Pensioner;
            var options = new List<PensionerOption> { new PensionerOption { Value = 0, Text = "Select Pensioner" } };
            if (pContext == null) return options;

            var pensioners = await pContext
                .Where(x => x.PDUId == PDUId && (!isActive || x.IsActiveClaimant))
                .OrderBy(x => x.PPOSystem)
                .ToListAsync();

            options.AddRange(pensioners.Select(item => new PensionerOption { Value = item.Id, Text = $"{item.PPOSystem}-{item.Claimant}" }));
            return options;
        }

        public async Task<List<Pensioner>?> GetPhysicalVerification(int PUIId) =>
            await _context.Pensioner
                .Include(r => r.Relation)
                .Where(p => p.IsActiveClaimant && p.PDUId == PUIId)
                .OrderBy(x => x.PPOSystem).ToListAsync();

        public async Task<IEnumerable<Pensioner>?> GetRestored(int PDUId) =>
            await _context.Pensioner
                .Include(c => c.Company)
                .Include(r => r.Relation)
                .Where(x => x.IsActiveClaimant && x.PDUId == PDUId && DateTime.Now.Year - x.DOB.Year >= 72)
                .OrderByDescending(p => p.DOB).ToListAsync();

        public async Task<List<Pensioner>?> GetSMSList(int PUIId) =>
            await _context.Pensioner
                .Include(x => x.Relation)
                .Where(x => x.IsActiveClaimant && x.PDUId == PUIId)
                .OrderBy(x => x.PPOSystem)
                .ToListAsync();

        public Task<(bool IsSaved, string Message)> Insert(Pensioner entity) => throw new NotImplementedException();

        public async Task<bool> RestoreNow(int PensionerId)
        {
            var p = await _context.Pensioner.FindAsync(PensionerId);
            if (p == null) return false;
            p.IsRestored = true;
            try
            {
                _context.Update(p);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<(bool IsSaved, string Message)> Update(Pensioner entity) => throw new NotImplementedException();

        public async Task<bool> UpdateAccountNumber(UpdateBranchDTO updateBranchDTO)
        {
            var r = await Table.FirstOrDefaultAsync(x => x.Id == updateBranchDTO.PensionerId);
            if (r == null) return false;
            try
            {
                r.BranchId = updateBranchDTO.BranchId;
                r.IBAN = updateBranchDTO.IBAN;
                r.AccountNumber = updateBranchDTO.AccountNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateSMSServiceStatus(int PensionerId, bool Status)
        {
            try
            {
                var p = await _context.Pensioner.FirstOrDefaultAsync(x => x.Id == PensionerId);
                if (p == null) return false;
                p.IsServiceActive = Status;
                _context.Update(p);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}