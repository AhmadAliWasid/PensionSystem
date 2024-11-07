using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using System.Linq.Expressions;

namespace WebAPI.Services
{
    public class PensionerService : IPensioner
    {
        private readonly ApplicationDbContext _context;

        public PensionerService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

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
                return (false, exc.Message.ToString());
            }
        }

        public async Task<(bool IsSaved, string Message)> AddToDBFromFile(List<PensionerUploadVM> pensionerUploadVMs)
        {
            var PDUContext = _context.PDUs;
            if (PDUContext == null)
                return (false, "Unable To Find PDUS");
            // check Companies
            var CompanyContext = _context.Company;
            if (CompanyContext == null)
                return (false, "Unable To Find Companies");
            var BankContext = _context.Banks;
            if (BankContext == null)
                return (false, "Unable To Find Banks");
            // check Branches
            var BranchContext = _context.Branches;
            if (BranchContext == null)
                return (false, "Unable To Find Branches in DB");

            var RelationContext = _context.Relations;
            if (RelationContext == null)
                return (false, "Unable to find relation id DB");

            var listPDUs = await PDUContext.ToListAsync();
            var listCompanies = await CompanyContext.ToListAsync();
            var listBanks = await BankContext.ToListAsync();
            var listBranches = await BranchContext.Include(x => x.Bank).ToListAsync();
            var listRelation = await RelationContext.ToListAsync();
            if (listPDUs != null && listCompanies != null && listBranches != null)
            {
                // local variables
                char cGender = 'M';
                int cRelationId = 0;
                string cSpouse = "";
                string cClaiment = "";
                try
                {
                    var pContext = _context.Pensioner;
                    if (pContext == null)
                        return (false, "Pension Table Does not Exist in DB");
                    // first we will check it is it already exist or not then
                    var pInDB = await pContext.Where(x => x.PDUId == pensionerUploadVMs.First().PDUId).ToListAsync();
                    // now i will add them to db
                    var listPensioners = new List<Pensioner>();
                    foreach (var item in pensionerUploadVMs)
                    {
                        // we will check the pdu Exists
                        var currentPDU = listPDUs.Where(x => x.Id == item.PDUId).FirstOrDefault();
                        if (currentPDU == null)
                            return (false, $"PDU Does not Exist for PPO # {item.PPONumber} Please Add the PDU {item.PDUId} First");
                        // we will check company
                        var currentCompany = listCompanies.Where(x => x.ShortName == item.CompanyShortName).FirstOrDefault();
                        if (currentCompany == null)
                            return (false, $"Company Does not Exist for PPO # {item.PPONumber} Please add this company {item.CompanyShortName} First");

                        // we will check Banks
                        var currentBank = listBanks.Where(x => x.ShortName == item.BankShortName).FirstOrDefault();
                        if (currentBank == null)
                            return (false, $"Bank Does not Exist for PPO # {item.PPONumber} Please add this Bank {item.BankShortName} First");

                        // we will check branch
                        var currentBranch = listBranches
                            .Where(x => x.Code.ToString() == item.BranchCode.ToString() && x.BankId == currentBank.Id)
                            .FirstOrDefault();
                        if (currentBranch == null)
                            return (false, $"Branch Does not Exist for PPO # {item.PPONumber} Please add this Branch {item.BranchCode} & IN Bank {item.BankShortName} First in Database");

                        // it mean all data is okay.
                        if (item.Claimant.ToUpper() != "SELF")
                        {
                            cGender = 'F';
                            cRelationId = 3;
                            cSpouse = "NA";
                            cClaiment = item.Claimant;
                        }
                        else
                        {
                            cGender = 'M';
                            cRelationId = 1;
                            cSpouse = item.Name;
                            cClaiment = item.Name;
                        }
                        // if the pension is already exist in db then leave ti
                        int pNumber = item.PPONumber;
                        var isExist = pInDB.Where(x => x.PPOSystem == pNumber).FirstOrDefault();
                        if (isExist == null)
                        {
                            listPensioners.Add(new Pensioner()
                            {
                                AccountNumber = item.AccountNumber,
                                AccountTitle = cClaiment,
                                Name = item.Name.Trim(),
                                Claimant = item.Claimant.Trim(),
                                AddedDate = DateTime.Now,
                                Address = "N/A",
                                BPS = item.BPS,
                                BranchId = currentBranch.Id,
                                DOB = DateTime.Now,
                                Designation = item.Designation.Trim(),
                                Gender = cGender,
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
                                RelationId = cRelationId,
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

                    }
                    await pContext.AddRangeAsync(listPensioners);
                    await _context.SaveChangesAsync();
                    return (true, "Saved Data Successfully!");
                }
                catch (Exception exc)
                {
                    return (false, $" {ExceptionHelper.GetDetail(exc)}");
                }
            }
            else
            {
                return (false, "PDU and Companies and Branches not Found");
            }
        }

        public Task<bool> Delete(Pensioner entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Pensioner?> Get(int Id)
        {
            var cContext = _context.Pensioner;
            if (cContext == null)
                return null;
            return await cContext.Include(c => c.Company)
                .Include(d => d.Branch)
                .Include(e => e.Branch.Bank)
                .Include(x => x.Relation)
                .FirstOrDefaultAsync(p => p.Id == Id);
        }

        public async Task<List<Pensioner>> Get(bool isActive = true)
        {
            var pContext = _context.Pensioner;
            if (pContext == null)
                return new List<Pensioner>();
            return await pContext.ToListAsync();
        }

        public async Task<List<Pensioner>?> GetActive(int PDUId)
        {
            var cContext = _context.Pensioner;
            if (cContext == null)
                return null;
            var active = await cContext
                .Include(c => c.Company)
                .Include(r => r.Relation)
                .Where(x => x.IsActiveClaimant == true && x.PDUId == PDUId).OrderBy(y => y.PPOSystem).ToListAsync();
            return active;
        }

        public async Task<List<Pensioner>?> GetActiveByCompany(int companyId, int PDUId)
        {
            var cContext = _context.Pensioner;
            if (cContext == null)
                return null;
            var active = await cContext
                .Where(x => x.IsActiveClaimant == true && x.CompanyId == companyId && x.PDUId == PDUId)
                .OrderBy(y => y.PPOSystem).ToListAsync();
            return active;
        }

        public async Task<List<Pensioner>?> GetAll(int PDUId)
        {
            var cContext = _context.Pensioner;
            if (cContext == null)
                return null;
            return await cContext.Include(b => b.Relation)
                .Include(x => x.Branch)
                .Include(c => c.Branch.Bank)
                .Where(p => p.PDUId == PDUId)
                .OrderBy(d => d.PPOSystem).ToListAsync();
        }

        public async Task<List<Pensioner>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            var walks = Table;
            var validPropertyNames = typeof(Pensioner).GetProperties().Select(p => p.Name);
            // Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                // Check if filterOn is a valid property/column name
                if (validPropertyNames.Contains(filterOn, StringComparer.OrdinalIgnoreCase))
                {
                    var paramExpr = Expression.Parameter(typeof(Pensioner), "x");
                    var propExpr = Expression.Property(paramExpr, filterOn);
                    var filterValueExpr = Expression.Constant(Convert.ChangeType(filterQuery, propExpr.Type)); // Convert filterQuery to the property type
                    var equalsExpr = Expression.Equal(propExpr, filterValueExpr);
                    var lambdaExpr = Expression.Lambda<Func<Pensioner, bool>>(equalsExpr, paramExpr);
                    walks = walks.Where(lambdaExpr);
                }
                else
                {
                    // Handle invalid filterOn parameter here (e.g., throw an exception or log an error)
                    throw new ArgumentException("Invalid filterOn parameter");
                }
            }
            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                // Check if sortBy is a valid property/column name
                if (validPropertyNames.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                {
                    var paramExpr = Expression.Parameter(typeof(Pensioner), "x");
                    var propExpr = Expression.Property(paramExpr, sortBy);
                    var lambdaExpr = Expression.Lambda<Func<Pensioner, double>>(propExpr, paramExpr);
                    walks = IsAscending ?? true ? walks.OrderBy(lambdaExpr) : (IQueryable<Pensioner>)walks.OrderByDescending(lambdaExpr);
                }
                else
                    throw new ArgumentException("Invalid sortBy parameter");
            }
            // Pagination
            int skipResult = (pageNumber - 1) * pageSize;
            return await walks.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public Task<List<Pensioner>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public async Task<Pensioner?> GetById(object id)
        {
            return await _context.Pensioner
                .Include(b => b.Branch)
                .Include(ba => ba.Branch.Bank)
                .Include(c => c.Company)
                .Include(r => r.Relation)
                .FirstOrDefaultAsync(x => x.Id == (int)id);
        }

        public async Task<Pensioner?> GetByPPOAndCNIC(int PPONumber, string CNIC)
        {
            var pContext = _context.Pensioner;
            if (pContext == null)
                return null;
            return await pContext.Where(x => x.PPOSystem == PPONumber && x.ClaimantCNIC == CNIC).FirstOrDefaultAsync();
        }

        public async Task<List<Pensioner>?> GetInActiveList(int PDUId)
        {
            var cContext = _context.Pensioner;
            if (cContext == null)
                return null;
            return await cContext
                .Include(b => b.Relation)
                .Include(x => x.Branch)
                .Include(c => c.Branch.Bank)
                .Include(co => co.Company)
                .Where(p => p.PDUId == PDUId && p.IsActiveClaimant == false)
                .OrderBy(d => d.PPOSystem).ToListAsync();
        }

        public async Task<int> GetMaxPageNumber(int companyId)
        {
            var pContext = _context.Pensioner;
            int MaxNumber = 0;
            if (pContext != null)
            {
                MaxNumber = await pContext.Where(x => x.CompanyId == companyId).MaxAsync(p => p.PageNumber);
            }
            return MaxNumber;
        }

        public async Task<IEnumerable<PensionerOption>> GetOptions(int PDUId, bool isActive = true)
        {
            var pContext = _context.Pensioner;
            var options = new List<PensionerOption>();

            if (pContext != null)
            {
                var pensioners = new List<Pensioner>();
                if (!isActive)
                {
                    pensioners = await pContext
                        .Where(p => p.PDUId == PDUId)
                        .OrderBy(x => x.PPOSystem)
                        .ToListAsync();
                }
                else
                {
                    pensioners = await pContext
                        .Where(x => x.IsActiveClaimant == true && x.PDUId == PDUId)
                        .OrderBy(x => x.PPOSystem)
                        .ToListAsync();
                }
                options.Add(new PensionerOption { Value = 0, Text = "Select Pensioner" });
                if (pensioners != null)
                {
                    foreach (var item in pensioners)
                    {
                        options.Add(new PensionerOption { Value = item.Id, Text = item.PPOSystem + "-" + item.Claimant });
                    }
                }
            }
            return options;
        }

        public async Task<List<Pensioner>?> GetPhysicalVerification(int PUIId)
        {
            var cContext = _context.Pensioner;
            if (cContext == null)
            {
                return null;
            }
            return await cContext.Include(r => r.Relation)
                .Where(p => p.IsActiveClaimant == true && p.PDUId == PUIId)
                .OrderBy(x => x.PPOSystem).ToListAsync();
        }

        public async Task<IEnumerable<Pensioner>?> GetRestored(int PDUId)
        {
            var pContext = _context.Pensioner;
            if (pContext != null)
            {
                var listPensioner = await pContext.Include(c => c.Company).Include(r => r.Relation)
                    .Where(x => x.IsActiveClaimant == true && x.PDUId == PDUId && DateTime.Now.Date.Year - x.DOB.Date.Year >= 72)
                    .OrderByDescending(p => p.DOB).ToListAsync();
                return listPensioner;
            }
            return null;
        }

        public async Task<List<Pensioner>?> GetSMSList(int PUIId)
        {
            var cContext = _context.Pensioner;
            if (cContext == null)
                return null;
            return await cContext.Include(x => x.Relation)
                .Where(x => x.IsActiveClaimant == true && x.PDUId == PUIId)
                .OrderBy(x => x.PPOSystem)
                .ToListAsync();
        }

        public Task<(bool IsSaved, string Message)> Insert(Pensioner entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RestoreNow(int PensionerId)
        {
            var p = await _context.Pensioner.FindAsync(PensionerId);
            if (p == null)
                return false;
            p.IsRestored = true;
            try
            {
                _context.Update(p);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<(bool IsSaved, string Message)> Update(Pensioner entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateSMSServiceStatus(int PensionerId, bool Status)
        {
            try
            {
                var cContext = _context.Pensioner;
                if (cContext == null)
                    return false;
                var p = cContext.Where(x => x.Id == PensionerId).FirstOrDefault();
                if (p == null)
                    return false;
                p.IsServiceActive = Status;
                cContext.Update(p);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}