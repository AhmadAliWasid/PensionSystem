using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Interfaces;
using WebAPI.Helpers;

namespace PensionSystem.Services
{
    public class PensionPaymentService : IPensionPayment
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _Configuration;

        public Task<IQueryable<PensionerPayment>> Table => throw new NotImplementedException();

        IQueryable<PensionerPayment> ICrud<PensionerPayment>.Table => throw new NotImplementedException();

        public PensionPaymentService(ApplicationDbContext applicationDbContext,
            IConfiguration configuration)
        {
            _context = applicationDbContext;
            _Configuration = configuration;
        }

        public async Task<string> GenerateDemandList(List<Pensioner> pensioners, int demandId, DateTime demandDate, int companyId)
        {
            var lastDemandDate = demandDate.AddMonths(-1);
            var listLastDemand = await GetByMonth(lastDemandDate);
            if (listLastDemand == null)
                return "Last Demand Records not found";
            bool certificateStatus = false;
            bool physicallyStatus = false;

            // first we will check the the amount is already verified make sure the date is not october or april
            List<(int PensionerId, bool CertificateStatus)> verificationStatusList = [];
            if (demandDate.Month != 10 && demandDate.Month != 4)
            {
                foreach (var item in listLastDemand)
                    verificationStatusList.Add((item.PensionerId, item.CertificateVerified));
            }

            // now we will do it for physical verification as well 01 & 04
            List<(int PensionerId, bool PhysicalStatus)> physicallyStatusList =
                [];
            if (demandDate.Month != 1 && demandDate.Month != 7)
            {
                foreach (var item in listLastDemand)
                    physicallyStatusList.Add((item.PensionerId, item.PhysicallyVerified));
            }
            try
            {

                foreach (var item in pensioners)
                {
                    // certificate verification checking
                    var recordC = verificationStatusList.Where(x => x.PensionerId == item.Id);
                    certificateStatus = recordC.Any() ? recordC.First().CertificateStatus : false;
                    // checking physical status
                    var recordP = physicallyStatusList.Where(x => x.PensionerId != item.Id);
                    physicallyStatus = recordP.Any() ? recordP.First().PhysicalStatus : false;

                    PensionerPayment pensionerPayment = new()
                    {
                        PensionerId = item.Id,
                        Month = demandDate,
                        MonthlyPension = item.MonthlyPension,
                        CMA = item.CMA,
                        OrderelyAllowence = item.OrderelyAllowence,
                        Total = item.Total,
                        Deduction = item.MonthlyRecovery,
                        NetPension = (item.Total - item.MonthlyRecovery),
                        ModifiedDate = DateTime.Now,
                        CertificateVerified = certificateStatus,
                        PhysicallyVerified = physicallyStatus,
                        MonthlyPensionDemandId = demandId
                    };
                    _context.Add(pensionerPayment);
                }
                await _context.SaveChangesAsync();
                return "Ok";
            }
            catch (Exception exc)
            {
                return exc.ToString();
            }
        }

        public async Task<List<PensionerPayment>> GetPaymentList(DateTime month)
        {
            return await _context.PensionerPayments.Where(x =>
            x.Month.ToString("yyyy-MM") == month.ToString("yyyy-MM")).ToListAsync();
        }

        public async Task<List<PensionerPayment>?> GetPensionPayments(int demandId)
        {
            var cContext = _context.PensionerPayments;
            if (cContext == null)
                return null;
            return await cContext
                .Include(p => p.Pensioner)
                .Include(b => b.Pensioner.Branch)
                .Include(ba => ba.Pensioner.Branch.Bank)
                .Include(c => c.Pensioner.Company)
                .Include(r => r.Pensioner.Relation)
                .Where(x => x.MonthlyPensionDemandId == demandId)
                .OrderBy(o => o.Pensioner.PPOSystem).ToListAsync();
        }

        public async Task<(bool isSaved, string Msg)> InitialSaving(DateTime month)
        {
            var activeList = await _context.Pensioner.Where(x => x.IsActiveClaimant == true).ToListAsync();
            if (activeList.Count > 0)
            {
                foreach (var item in activeList)
                {
                    PensionerPayment pensionerPayment = new PensionerPayment()
                    {
                        PensionerId = item.Id,
                        Month = month,
                        MonthlyPension = item.MonthlyPension,
                        CMA = item.CMA,
                        OrderelyAllowence = item.OrderelyAllowence,
                        Total = item.Total,
                        Deduction = 0,
                        NetPension = (item.MonthlyPension + item.CMA + item.OrderelyAllowence),
                        ModifiedDate = DateTime.Now,
                        CertificateVerified = false,
                        PhysicallyVerified = false
                    };
                    _context.PensionerPayments.Add(pensionerPayment);
                }
                try
                {
                    await _context.SaveChangesAsync();
                    return (true, "");
                }
                catch (Exception exc)
                {
                    return (false, exc.Message.ToString());
                }
            }
            else
            {
                return (true, "");
            }
        }

        public async Task<List<PensionerPayment>?> GetByDemand(int DemandId)
        {
            var CContext = _context.PensionerPayments;
            if (CContext == null)
                return null;
            return await CContext.Where(x => x.MonthlyPensionDemandId == DemandId).ToListAsync();
        }

        public async Task<List<PensionPaymentHistoryVM>?> GetByPensionerId(int PensionerId)
        {
            var cContext = _context.PensionerPayments;
            if (cContext == null)
                return null;
            var listDemands = await cContext
                .Include(y => y.MonthlyPensionDemand)
                .Where(c => c.PensionerId == PensionerId).ToListAsync();
            var vm = new List<PensionPaymentHistoryVM>();
            if (listDemands != null && listDemands.Count > 0)
            {
                foreach (var item in listDemands)
                {
                    vm.Add(new PensionPaymentHistoryVM
                    {
                        DemandNo = item.MonthlyPensionDemand.Number,
                        DemandDate = item.MonthlyPensionDemand.Date,
                        Description = "Monthly",
                        Month = item.Month,
                        MP = item.MonthlyPension,
                        CMA = item.CMA,
                        Orderly = item.OrderelyAllowence,
                        Total = item.Total,
                        Paid = item.NetPension
                    });
                }
            }
            var aContext = _context.ArrearsPayments;
            if (aContext != null)
            {
                var listArrears = await aContext.Include(c => c.ArrearsDemand).Where(x => x.PensionerId == PensionerId).ToListAsync();
                if (listArrears != null && listArrears.Count > 0)
                {
                    foreach (var item in listArrears)
                    {
                        vm.Add(new PensionPaymentHistoryVM
                        {
                            DemandNo = item.ArrearsDemand.Number,
                            DemandDate = item.ArrearsDemand.Date,
                            Description = item.Description + "\n" + UserFormat.GetDate(item.FromMonth) + " To " + UserFormat.GetDate(item.ToMonth),
                            Month = item.FromMonth,
                            MP = item.MP,
                            CMA = item.CMA,
                            Orderly = item.Orderly,
                            Total = item.Total
                        });
                    }
                }
            }
            return vm.OrderBy(x => x.Month).ToList();
        }

        public async Task<List<PensionerPayment>?> GetByMonth(DateTime dateTime)
        {
            var CContext = _context.PensionerPayments;
            if (CContext == null)
                return null;
            return await CContext.Include(c => c.Pensioner).Include(p => p.Pensioner.Company)
                .Where(x => x.Month.Month == dateTime.Month && x.Month.Year == dateTime.Year).ToListAsync();
        }

        public async Task<bool> VerifyPhysically(int id)
        {
            var cContext = _context.PensionerPayments;
            if (cContext == null)
                return false;
            var entity = await cContext.FindAsync(id);
            if (entity == null)
                return false;
            entity.PhysicallyVerified = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyCertificate(int id)
        {
            var cContext = _context.PensionerPayments;
            if (cContext == null)
                return false;
            var entity = await cContext.FindAsync(id);
            if (entity == null)
                return false;
            entity.CertificateVerified = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public PensionerPayment GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool IsSaved, string Message)> Insert(PensionerPayment entity)
        {
            throw new NotImplementedException();
        }

        public Task<(bool IsSaved, string Message)> Update(PensionerPayment entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(PensionerPayment entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<PensionerPayment>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        Task<PensionerPayment?> ICrud<PensionerPayment>.GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PensionerPayment>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }
    }
}