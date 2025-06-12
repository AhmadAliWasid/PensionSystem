using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using WebAPI.Helpers;

namespace PensionSystem.Services
{
    public class ArrearsPaymentService(ApplicationDbContext applicationDbContext) : IArrearsPayment
    {
        private readonly ApplicationDbContext _context = applicationDbContext;

        public IQueryable<ArrearsPayment> Table => _context.ArrearsPayments;

        public async Task<ArrearsPayment>? Get(int Id)
        {
            var list = _context.ArrearsPayments;
            if (list != null)
            {
                var arrearsPayment = await list.FindAsync(Id);
                if (arrearsPayment != null)
                    return arrearsPayment;
            }
            return new ArrearsPayment();
        }

        public async Task<List<ArrearsPayment>> GetByDemand(int demandId)
        {
            var contextPayment = _context.ArrearsPayments;
            if (contextPayment != null)
            {
                return await contextPayment
                    .Include(p => p.Pensioner)
                    .Include(r => r.Pensioner.Relation)
                    .Include(b => b.Pensioner.Branch)
                    .Include(ba => ba.Pensioner.Branch.Bank)
                    .Where(x => x.ArrearsDemandId == demandId)
                    .OrderBy(z => z.Pensioner.PPOSystem).ThenBy(d => d.FromMonth).ToListAsync();
            }
            else
            {
                return new List<ArrearsPayment>();
            }
        }

        public async Task<List<ArrearsPayment>> GetByDemandForPayment(int demandId, int BankId)
        {
            var contextPayment = _context.ArrearsPayments;
            if (contextPayment != null)
            {
                return await contextPayment
                    .Include(p => p.Pensioner)
                    .Include(b => b.Pensioner.Branch)
                    .Where(x => x.ArrearsDemandId == demandId && x.Total > 0 && x.Pensioner.Branch.BankId == BankId)
                    .OrderBy(z => z.Pensioner.PPOSystem)
                    .ThenBy(d => d.FromMonth).ToListAsync();
            }
            else
            {
                return new List<ArrearsPayment>();
            }
        }

        public async Task<(bool Valid, string Msg)> IsValid(int PensionerId, DateTime StartingDate, DateTime EndingDate)
        {
            var arrearContext = _context.ArrearsPayments;
            bool isValid = true;
            string Msg = "";
            // first check is it demanded before
            if (arrearContext != null)
            {
                var sArrearsPayments = await arrearContext.Include(a => a.ArrearsDemand)
                    .Where(x =>
                    ((x.FromMonth.Date <= StartingDate.Date && x.ToMonth >= StartingDate.Date) || (x.ToMonth.Date <= EndingDate.Date && x.ToMonth.Date >= EndingDate.Date)) && x.PensionerId == PensionerId).FirstOrDefaultAsync();
                if (sArrearsPayments != null)
                {
                    Msg += "<br>Already Demanded in Month : " + UserFormat.GetDate(sArrearsPayments.FromMonth);
                    if (sArrearsPayments.ArrearsDemand != null)
                    {
                        Msg += "Via Demand No. " + sArrearsPayments.ArrearsDemand.Number + " On Dated : " + UserFormat.GetDate(sArrearsPayments.ArrearsDemand.Date);
                    }
                    isValid = false;
                }
            }
            // check it hbl arrears
            var hblArrearContext = _context.HBLArrears;
            if (hblArrearContext != null)
            {
                var sHBLPaid = await hblArrearContext.Where(
                    x =>
                    ((x.FromMonth.Date >= StartingDate.Date && StartingDate.Date <= x.ToMonth.Date) || (x.FromMonth.Date >= EndingDate && EndingDate.Date <= x.ToMonth.Date)) && x.PensionerId == PensionerId).FirstOrDefaultAsync();
                if (sHBLPaid != null)
                {
                    Msg += "<br>HBL Arrears Paid From: " + UserFormat.GetDate(sHBLPaid.FromMonth) + " To " + UserFormat.GetDate(sHBLPaid.ToMonth);
                    Msg += "<br> Month: " + UserFormat.GetDate(sHBLPaid.Month) + " &  Amount : " + UserFormat.GetAmount(sHBLPaid.Amount);
                    isValid = false;
                }
            }

            // check it in HBL monthly
            var mHBLContext = _context.HBLPayments;
            if (mHBLContext != null)
            {
                var months = Calculation.GetMonths(StartingDate, EndingDate);
                if (months.Count > 0)
                {
                    foreach (var item in months)
                    {
                        var singlePayment = await mHBLContext.Where(x => (x.Month.Date.Month == item.Item1 && x.Month.Year == item.Item2) && x.PensionerId == PensionerId).FirstOrDefaultAsync();
                        if (singlePayment != null)
                        {
                            Msg += "<br>Monlthy Paid via HBL: " + UserFormat.GetDate(singlePayment.Month) + " Amount Rs : " + UserFormat.GetAmount(singlePayment.Total);
                            isValid = false;
                            break;
                        }
                    }
                }
            }
            return (isValid, Msg);
        }

        /// <summary>
        /// Deleting Record of arrears
        /// </summary>
        /// <param name="DemandId"></param>
        /// <returns></returns>
        public async Task<JsonResponseHelper> ClearDemand(int DemandId)
        {
            var arrearsContext = _context.ArrearsPayments;
            JsonResponseHelper js = new JsonResponseHelper();
            if (arrearsContext == null)
            {
                js.RText = "No context found";
                return js;
            }
            // first we will make sure the demand is not locked
            var demandContext = _context.ArrearsDemands;
            if (demandContext == null)
            {
                js.RText = "Unable to Find DB";
                return js;
            }
            var demand = await demandContext.FindAsync(DemandId);
            if (demand == null)
            {
                js.RText = "No Record Found in DB";
                return js;
            }
            if (demand.IsPaid)
            {
                js.RText = "Demand is Paid and can not be changed";
                return js;
            }
            var listArrearsPayment = await arrearsContext.Where(x => x.ArrearsDemandId == DemandId).ToListAsync();
            if (listArrearsPayment == null)
            {
                js.RText = "No Record Found";
                return js;
            }
            try
            {
                _context.RemoveRange(listArrearsPayment);
                await _context.SaveChangesAsync();
                js.RText = "Deleted Successfully!";
                js.RCode = 1;
            }
            catch (Exception exc)
            {
                js.RText = exc.Message.ToString();
            }
            return js;
        }

        public async Task<List<ArrearsPayment>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table.ToListAsync();
        }

        public async Task<List<ArrearsPayment>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table.ToListAsync();
        }

        public async Task<ArrearsPayment?> GetById(object id)
        {
            var r = await Table.Where(x => x.Id == (int)id).FirstOrDefaultAsync();
            return r;
        }

        public async Task<(bool IsSaved, string Message)> Insert(ArrearsPayment entity)
        {
            try
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception ex)
            {
                return (false, ex.Message + " Detail  : " + ex.InnerException.Message.ToString());
            }
        }

        public async Task<(bool IsSaved, string Message)> Update(ArrearsPayment entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message + "Detail : " + exc.InnerException.Message.ToString());
            }
        }

        public async Task<bool> Delete(ArrearsPayment entity)
        {
            try
            {
                _context.ArrearsPayments.Remove(entity);
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