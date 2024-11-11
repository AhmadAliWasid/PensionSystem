using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Interfaces;

namespace WebAPI.Services
{
    public class WWFSanctionService(ApplicationDbContext applicationDbContext) : IWWFSanction
    {
        private readonly ApplicationDbContext _context = applicationDbContext;

        public IQueryable<WWFSanction> Table => _context.WWFSanctions;

        public async Task<bool> Delete(WWFSanction entity)
        {
            try
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<WWFSanction>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table.OrderByDescending(X => X.Date).ToListAsync();
        }

        public async Task<List<WWFSanction>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table
                .Where(y => y.PDUId == PDUId)
                .OrderByDescending(X => X.Date).ToListAsync();
        }

        public async Task<WWFSanction?> GetById(object id)
        {
            try
            {
                return await _context.WWFSanctions.FindAsync(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions()
        {
            var options = new List<SelectOptions>();
            var relations = await Table.ToListAsync();
            options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
            if (relations != null)
            {
                foreach (var item in relations)
                {
                    options.Add(new SelectOptions { Value = item.Id, Text = item.Claimant });
                }
            }
            return options;
        }

        public async Task<(bool IsSaved, string Message)> Insert(WWFSanction entity)
        {
            try
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message);
            }
        }

        public async Task<(bool IsSaved, string Message)> Update(WWFSanction entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message);
            }
        }
    }
}
