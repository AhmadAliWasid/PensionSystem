using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Interfaces;

namespace WebAPI.Services
{
    public class WWFReimbursmentService(ApplicationDbContext applicationDbContext) : IWWFReimbursment
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        public IQueryable<WGReimbursment> Table => _applicationDbContext.WGReimbursments;

        public async Task<bool> Delete(WGReimbursment entity)
        {
            try
            {
                _applicationDbContext.WGReimbursments.Remove(entity);
                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<WGReimbursment>> GetAll(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table.Include(x => x.WWFSanction).ToListAsync();
        }

        public async Task<List<WGReimbursment>> GetAll(int PDUId = 0, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool? IsAscending = null, int pageNumber = 1, int pageSize = 1000)
        {
            return await Table
                .Include(x => x.WWFSanction)
                .Where(p => p.WWFSanction.PDUId == PDUId)
                .OrderByDescending(c => c.To).ToListAsync();
        }

        public async Task<WGReimbursment?> GetById(object id)
        {
            try
            {
                var primaryKey = (int)id;
                return await _applicationDbContext.WGReimbursments.Where(x => x.Id == primaryKey).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<(bool IsSaved, string Message)> Insert(WGReimbursment entity)
        {
            try
            {
                await _applicationDbContext.AddAsync(entity);
                await _applicationDbContext.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message.ToString());
            }
        }

        public async Task<(bool IsSaved, string Message)> Update(WGReimbursment entity)
        {
            try
            {
                _applicationDbContext.Update(entity);
                await _applicationDbContext.SaveChangesAsync();
                return (true, "ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message.ToString());
            }
        }
    }
}
