using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Entities.Models;
using WebAPI.Helpers;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ConversionCasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConversionCasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ConversionCases
        public IActionResult Index()
        {
            return View();
        }

        // GET: ConversionCases/Details/5
        public async Task<IActionResult> Load()
        {
            return PartialView("_List", await _context.ConversionCases.OrderByDescending(x => x.DispatchDate).ToListAsync());
        }

        public IActionResult FullCase()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Crud(int id = 0)
        {
            var conversionCase = await _context.ConversionCases.Where(x => x.Id == id).FirstOrDefaultAsync();
            return PartialView("_CreateUpdate", conversionCase);
        }

        // POST: ConversionCases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<JsonResult> Save(ConversionCase conversionCase)
        {
            JsonResponseHelper helper = new();

            if (ModelState.IsValid)
            {
                try
                {
                    if (conversionCase.Id == 0)
                        await _context.AddAsync(conversionCase);
                    else
                        _context.Update(conversionCase);
                    await _context.SaveChangesAsync();

                    helper.RText = "ok";
                    helper.RCode = 1;
                }
                catch (Exception exc)
                {
                    helper.RText = exc.Message.ToString();
                }
            }
            else
            {
                foreach (var item in ModelStateHelper.Errors(ModelState))
                {
                    helper.RText += item;
                }
            }
            return Json(helper);
        }

        [HttpPost]
        public IActionResult GetGetFullCase(Pensioner pensioner)
        {
            return PartialView("FilledCase", pensioner);
        }
    }
}