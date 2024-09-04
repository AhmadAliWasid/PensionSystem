using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Entities.Models;
using WebAPI.Interfaces;

namespace WebAPI.Services
{
    public class WWFSanctionService : IWWFSanction
    {
        private readonly ApplicationDbContext _context;
        public WWFSanctionService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
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
            return await Table.ToListAsync();
        }

        public async Task<List<WWFSanction>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table.ToListAsync();
        }

        public Task<WWFSanction?> GetById(object id)
        {
            throw new NotImplementedException();
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
