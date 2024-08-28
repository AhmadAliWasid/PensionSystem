using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Services
{
    public class PDUService : IPDU
    {
        private readonly ApplicationDbContext _context;

        public PDUService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<PDU?> Get(int Id)
        {
            return await _context.PDUs.FindAsync(Id);
        }

        public Task<List<PDU>> GetAll()
        {
            var cContext = _context.PDUs;
            return cContext.ToListAsync();
        }

        public async Task<IEnumerable<SelectOptions>> GetOptions()
        {
            var rContext = _context.PDUs;
            var options = new List<SelectOptions>();

            if (rContext != null)
            {
                var relations = await rContext.ToListAsync();
                options.Add(new SelectOptions { Value = 0, Text = "Select Option" });
                if (relations != null)
                {
                    foreach (var item in relations)
                    {
                        if (item.Name != null)
                            options.Add(new SelectOptions { Value = item.Id, Text = item.ShortName });
                    }
                }
            }
            return options;
        }

        public async Task<(bool isSaved, string Message)> Save(PDU pDU)
        {
            var cContext = _context.PDUs;
            if (cContext == null)
            {
                return (false, "Unable to Find DB");
            }
            try
            {
                if (pDU.Id == 0)
                {
                    await cContext.AddAsync(pDU);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var returnPDU = await _context.PDUs.FindAsync(pDU.Id);
                    if (returnPDU != null)
                    {
                        returnPDU.Name = pDU.Name;
                        returnPDU.ShortName = pDU.ShortName;
                        returnPDU.AMStamp = pDU.AMStamp;
                        returnPDU.DMStamp = pDU.DMStamp;
                        returnPDU.BaseStamp = pDU.BaseStamp;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return (false, "Unable to find");
                    }
                }
                return (true, "Ok");
            }
            catch (Exception exc)
            {
                return (false, exc.ToString());
            }
        }
    }
}