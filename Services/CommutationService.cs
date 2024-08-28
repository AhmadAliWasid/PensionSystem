using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;

namespace PensionSystem.Services
{
    public class CommutationService(ApplicationDbContext applicationDbContext) : ICommutation
    {
        private readonly ApplicationDbContext _context = applicationDbContext;

        public IQueryable<Commutation> Table => _context.Set<Commutation>();

        public Task<bool> Delete(Commutation entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Commutation>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Commutation>> GetAll(
            int PDUId = 0,
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool? IsAscending = null,
            int pageNumber = 1,
            int pageSize = 1000)
        {
            return await Table
                .Include(x => x.Pensioner)
                .OrderByDescending(x => x.Month)
                .Take(pageSize).ToListAsync();
        }

        public Task<Commutation?> GetById(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Commutation>> GetCommutations()
        {
            var CommuationContext = _context.Commutations;
            var listCommutations = new List<Commutation>();
            if (CommuationContext != null)
            {
                listCommutations = await CommuationContext.Include(x => x.Pensioner).ToListAsync();
            }
            return listCommutations;
        }

        public async Task<List<Commutation>> GetCommutations(DateOnly Month)
        {
            var cCommutations = _context.Commutations;
            if (cCommutations == null)
            {
                return new List<Commutation>();
            }
            return await cCommutations
                .Include(p => p.Pensioner)
                .Include(x => x.Pensioner.Company)
                .Include(r => r.Pensioner.Relation)
                .Where(x => x.Month.Month == Month.Month && x.Month.Year == Month.Year)
                .ToListAsync();
        }

        public async Task<List<Commutation>> GetCommutationsByDates(DateTime startingDate, DateTime endingDate)
        {
            var commutations = await GetCommutations();
            return commutations.Where(x => x.Month >= startingDate && x.Month <= endingDate).ToList();
        }

        public async Task<(bool IsSaved, string Message)> Insert(Commutation entity)
        {
            try
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {

                return (false, exc.Message + " Error: " + exc.InnerException.Message);
            }
        }

        public async Task<(bool IsSaved, string Message)> Update(Commutation entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {

                return (false, exc.Message + " Error: " + exc.InnerException.Message);
            }
        }
    }
}