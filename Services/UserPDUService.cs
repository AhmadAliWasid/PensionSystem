using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;

namespace PensionSystem.Services
{
    public class UserPDUService : IUserPDU
    {
        private readonly ApplicationDbContext _context;

        public UserPDUService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserPDU>> GetAll()
        {
            return await _context.UserPDUs.Include(x => x.User).Include(c => c.PDU).ToListAsync();
        }

        public async Task<UserPDU?> GetByUserId(string UserId)
        {
            var cContext = _context.UserPDUs;
            return await cContext
                .Include(p => p.PDU)
                .Where(x => x.UserId == UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<(bool IsSaved, string Message)> Save(UserPDU userPDU)
        {
            try
            {
                var cContext = _context.UserPDUs;
                if (cContext == null)
                    return (false, "Error");
                await cContext.AddAsync(userPDU);
                await _context.SaveChangesAsync();
                return (true, "Ok");
            }
            catch (Exception exc)
            {
                return (false, exc.Message.ToString());
            }
        }
    }
}