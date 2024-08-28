using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;

namespace PensionSystem.Controllers
{
    public class SMSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SMSController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Load()
        {
            return PartialView("_List", await _context.MessagesHistories
                .Include(p => p.Pensioner).ToListAsync());
        }
    }
}