using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Helpers;

namespace PensionSystem.Services
{
    public class HBLPaymentService(ApplicationDbContext applicationDbContext) : IHBLPayments
    {
        private readonly ApplicationDbContext _context = applicationDbContext;
        public async Task<List<HBLPaymentPensioner>?> GetAllPensioners(DateTime startingDate, DateTime endingDate, int PDUId)
        {
            var HBLPensioners = _context.HBLPayments;

            List<HBLPaymentPensioner> listPensioners = [];
            if (HBLPensioners != null)
            {
                var listPaymentsOnly = await HBLPensioners
                    .Include(x => x.Pensioner)
                    .Include(r => r.Pensioner.Relation)
                    .Include(c => c.Pensioner.Company)
                    .Include(ch => ch.Cheque)
                    .Where(z => z.Month >= startingDate && z.Month <= endingDate && z.Cheque.PDUId == PDUId)
                    .GroupBy(x => x.Pensioner.Id)
                    .Select(g => g.First())
                    .ToListAsync();
                foreach (var item in listPaymentsOnly)
                {
                    string lClaimant = "";
                    string shortRelation = "";

                    if (item.Pensioner != null && item.Pensioner.Relation != null)
                    {
                        shortRelation += item.Pensioner.Relation.Short;
                        lClaimant += item.Pensioner.Claimant;
                        if (item.Pensioner.Relation.Short == "S/O" || item.Pensioner.Relation.Short == "D/O")
                        {
                            lClaimant += " " + item.Pensioner.Relation.Short + " " + item.Pensioner.FatherName + " " + "Ex. " + item.Pensioner.Designation;
                        }
                        else
                        {
                            lClaimant += " " + item.Pensioner.Relation.Short + " " + item.Pensioner.Spouse + "Ex. " + item.Pensioner.Designation;
                        }
                    }

                    listPensioners.Add(new HBLPaymentPensioner
                    {
                        PensionerId = item.PensionerId,
                        PPOSystem = item.Pensioner.PPOSystem,
                        Claimant = lClaimant,
                        CompanyId = item.Pensioner.CompanyId,
                        CompanyName = item.Pensioner.Company.Name,
                        CompanyNumber = item.Pensioner.Company.Order,
                        Designation = item.Pensioner.Designation,
                        ShortRel = shortRelation
                    });
                }
            }
            // now adding arrears list to the main
            var HBLArrears = _context.HBLArrears;
            if (HBLArrears != null)
            {
                var listhHBLArrears = await HBLArrears
                    .Include(p => p.Pensioner)
                    .Include(c => c.Pensioner.Company)
                    .Include(ch => ch.Cheque)
                    .Where(x => x.Month >= startingDate && x.Month <= endingDate && x.Cheque.PDUId == PDUId)
                    .ToListAsync();
                if (listhHBLArrears.Count > 0)
                {
                    foreach (var item in listhHBLArrears)
                    {
                        var selectedPensioner = listPensioners.Where(x => x.PensionerId == item.PensionerId).FirstOrDefault();
                        if (selectedPensioner == null)
                        {
                            listPensioners.Add(new HBLPaymentPensioner
                            {
                                PensionerId = item.PensionerId,
                                PPOSystem = item.Pensioner.PPOSystem,
                                Claimant = item.Pensioner.Claimant,
                                Name = item.Pensioner.Name,
                                CompanyId = item.Pensioner.CompanyId,
                                CompanyName = item.Pensioner.Company.Name,
                                CompanyNumber = item.Pensioner.Company.Order,
                                Designation = item.Pensioner.Designation
                            });
                        }
                    }
                }
            }
            // now adding commutation list
            var HBLCommutations = _context.Commutations;
            if (HBLCommutations != null)
            {
                var listHBLCommutations = await HBLCommutations
                    .Include(p => p.Pensioner)
                    .Include(c => c.Pensioner.Company)
                    .Include(ch => ch.Cheque)
                        .Where(x => x.Month >= startingDate && x.Month <= endingDate && x.Cheque.PDUId == PDUId)
                        .ToListAsync();
                foreach (var item in listHBLCommutations)
                {
                    // first make sure the pensioner is not exist in list
                    if (!(listPensioners.Exists(x => x.PensionerId == item.PensionerId)))
                    {
                        listPensioners.Add(new HBLPaymentPensioner
                        {
                            PensionerId = item.PensionerId,
                            PPOSystem = item.Pensioner.PPOSystem,
                            Claimant = item.Pensioner.Claimant,
                            Name = item.Pensioner.Name,
                            CompanyId = item.Pensioner.CompanyId,
                            CompanyName = item.Pensioner.Company.Name,
                            CompanyNumber = item.Pensioner.Company.Order,
                            Designation = item.Pensioner.Designation
                        });
                    }
                }
            }
            return listPensioners;
        }
        public async Task<List<HBLPayment>?> GetByChequeId(int chequeId)
        {
            var c = _context.HBLPayments;
            if (c == null)
                return [];
            return await c
                .Include(p => p.Pensioner)
                .Include(x => x.Branch)
                .Include(ba => ba.Branch.Bank)
                .Include(c => c.Pensioner.Company)
                .Where(x => x.ChequeId == chequeId)
                .OrderBy(y => y.Pensioner.PPOSystem).ToListAsync();
        }
        public async Task<List<HBLPayment>?> GetByMonth(DateTime month)
        {
            var pensionersPayments = _context.HBLPayments;
            if (pensionersPayments != null)
            {
                return await pensionersPayments
                    .Include(p => p.Pensioner)
                    .Include(cp => cp.Pensioner.Company)
                    .Include(c => c.Branch)
                    .Include(b => b.Branch.Bank)
                    .Include(r => r.Pensioner.Relation)
                    .Where(x => x.Month.Month == month.Month && x.Month.Year == month.Year)
                    .OrderBy(a => a.Pensioner.PPOSystem)
                    .ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<List<HBLPayment>?> GetByMonth(DateTime month, int PDUId)
        {
            var pensionersPayments = _context.HBLPayments;
            if (pensionersPayments != null)
            {
                return await pensionersPayments
                    .Include(p => p.Pensioner)
                    .Include(cp => cp.Pensioner.Company)
                    .Include(c => c.Branch)
                    .Include(b => b.Branch.Bank)
                    .Include(r => r.Pensioner.Relation)
                    .Include(ch => ch.Cheque)
                    .Where(x => x.Month.Month == month.Month && x.Month.Year == month.Year && x.Cheque.PDUId == PDUId)
                    .OrderBy(a => a.Pensioner.PPOSystem)
                    .ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<List<HBLPaymentHistoryPensionerVM>?> GetByPensionerId(int PensionerId)
        {
            var cContext = _context.HBLPayments;
            if (cContext == null)
                return null;
            var listHBL = await cContext
                .Include(b => b.Branch)
                .Include(ba => ba.Branch.Bank)
                .Include(c => c.Cheque)
                .Where(x => x.PensionerId == PensionerId).ToListAsync();
            var HBLPaymentHistoryPensionerVMList = new List<HBLPaymentHistoryPensionerVM>();
            if (listHBL.Count > 0)
            {
                foreach (var item in listHBL)
                {
                    HBLPaymentHistoryPensionerVMList.Add(new HBLPaymentHistoryPensionerVM
                    {
                        ChequeNo = item.Cheque.Number,
                        Date = item.Cheque.Date,
                        AccountNumber = item.Branch.Code + "-" + item.AccountNumber,
                        Description = "Monthly",
                        Branch = item.Branch.Bank.ShortName + " ( " + item.Branch.Name + " )",
                        Month = item.Month,
                        MP = item.MonthlyPension,
                        CMA = item.CMA,
                        Orderly = item.OrderlyAllowance,
                        Deduction = item.Deduction,
                        Total = item.Total
                    });
                }
            }
            var aContext = _context.HBLArrears;
            if (aContext != null)
            {
                var listArrears = await aContext
                    .Include(c => c.Cheque)
                    .Include(a => a.Branch)
                    .Include(b => b.Branch.Bank)
                    .Where(x => x.PensionerId == PensionerId).ToListAsync();
                if (listArrears != null && listArrears.Count > 0)
                {
                    foreach (var item in listArrears)
                    {
                        HBLPaymentHistoryPensionerVMList.Add(new HBLPaymentHistoryPensionerVM
                        {
                            ChequeNo = item.Cheque.Number,
                            Date = item.Cheque.Date,
                            Month = item.Month,
                            Branch = item.Branch.Bank.ShortName + " (" + item.Branch.Name + ")",
                            AccountNumber = item.Branch.Code + "-" + item.AccountNumber,
                            Description = item.Description + " " + UserFormat.GetDate(item.FromMonth) + " To " + UserFormat.GetDate(item.ToMonth),
                            MP = 0,
                            CMA = 0,
                            Orderly = 0,
                            Deduction = 0,
                            Total = item.Amount
                        });
                    }
                }
            }
            // commutation as well
            var comContext = _context.Commutations;
            if (comContext != null)
            {
                var listCommutation = await comContext
                    .Include(c => c.Cheque)
                    .Where(x => x.PensionerId == PensionerId).ToListAsync();
                if (listCommutation != null && listCommutation.Count > 0)
                {
                    foreach (var item in listCommutation)
                    {
                        HBLPaymentHistoryPensionerVMList.Add(new HBLPaymentHistoryPensionerVM
                        {
                            ChequeNo = item.Cheque.Number,
                            Date = item.Cheque.Date,
                            Month = item.Month,
                            AccountNumber = "Via Cheque",
                            Description = "Commutations",
                            MP = 0,
                            CMA = 0,
                            Orderly = 0,
                            Deduction = 0,
                            Total = item.Amount
                        });
                    }
                }
            }
            return [.. HBLPaymentHistoryPensionerVMList.OrderByDescending(x => x.Month)];
        }

        public async Task<List<HBLPayment>?> GetConsolidatedPensioner(DateTime startingDate, DateTime endingDate, int PDUId)
        {
            var listPensioners = _context.HBLPayments;
            if (listPensioners != null)
            {
                var listPaymentsOnly = await listPensioners
                    .Include(x => x.Pensioner)
                    .Include(c => c.Cheque)
                    .Where(y => y.Month >= startingDate && y.Month <= endingDate && y.Cheque.PDUId == PDUId)
                    .ToListAsync();
                return listPaymentsOnly;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<HBLPayment>?> GetHBLPayments(DateOnly dateOnly)
        {
            var pensionersPayments = _context.HBLPayments;
            if (pensionersPayments != null)
            {
                return await pensionersPayments
                    .Include(p => p.Pensioner)
                    .Include(c => c.Pensioner.Company)
                    .Where(x => x.Month.Month == dateOnly.Month && x.Month.Year == dateOnly.Year)
                    .ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<string> PayDemandByCheque(int chequeId, int DemandId, int BankId)
        {
            var cContext = _context;
            if (cContext == null)
                return "Error";
            try
            {
                var myContext = _context
                    .PensionerPayments;
                if (myContext == null)
                    return "Error";
                var monthlyPensionDemand = await _context
                    .MonthlyPensionDemands.FindAsync(DemandId);
                if (monthlyPensionDemand == null)
                    return "Error";
                // making sure the cheque exists

                var demandList = await myContext
                    .Include(p => p.Pensioner)
                    .Include(b => b.Pensioner.Branch)
                    .Where(x => x.MonthlyPensionDemandId == DemandId
                                                         && x.Pensioner.Branch.BankId == BankId
                                                         && x.CertificateVerified
                                                         && x.PhysicallyVerified
                                                         && x.NetPension > 0)
                                                         .ToListAsync();
                if (demandList == null)
                    return "Error";
                var hblContext = _context.HBLPayments;
                if (hblContext == null)
                    return "Error";
                var chContext = _context.Cheque;
                if (chContext == null)
                    return "Error";
                var cheque = await chContext.FindAsync(chequeId);
                if (cheque == null)
                    return "Unable to find cheque";

                foreach (var item in demandList)
                {
                    hblContext.Add(new HBLPayment
                    {
                        ChequeId = chequeId,
                        PensionerId = item.PensionerId,
                        AccountNumber = item.Pensioner.AccountNumber,
                        MonthlyPension = item.MonthlyPension,
                        CMA = item.CMA,
                        OrderlyAllowance = item.OrderelyAllowence,
                        Deduction = item.Deduction,
                        Total = item.NetPension,
                        Month = cheque.Date,
                        BranchId = item.Pensioner.BranchId,
                        CreatedDate = DateTime.Now,
                        ModifiedOn = DateTime.Now
                    });
                }
                await cContext.SaveChangesAsync();
                return "Ok";
            }
            catch (Exception exc)
            {
                return exc.ToString();
            }
        }
    }
}