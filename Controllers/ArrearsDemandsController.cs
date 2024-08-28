using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Entities.Models;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ArrearsDemandsController(ApplicationDbContext context, IArrearsDemand arrearsDemand, SessionHelper sessionHelper, IMapper mapper, IArrearsPayment arrearsPayment) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IArrearsDemand _arrearsDemand = arrearsDemand;
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly IMapper _mapper = mapper;
        private readonly IArrearsPayment _arrearsPayment = arrearsPayment;

        // GET: ArrearsDemands
        public async Task<IActionResult> Index()
        {
            ArrearsDemandViewModel model = new();
            var arrearsContext = _context.ArrearsPayments;
            if (arrearsContext != null)
            {
                model.ArrearsDemands = await _arrearsDemand.GetArrears(_sessionHelper.GetUserPDUId());
                model.ArrearsPayments = await arrearsContext.ToListAsync();
            }
            return View(model);
        }
        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                return PartialView("_Crud", new CreateArrearDemandModel());
            }
            else
            {
                var r = await _arrearsDemand.Get(id);
                if (r != null)
                {
                    var record = _mapper.Map<CreateArrearDemandModel>(r);
                    return PartialView("_Crud", record);
                }
                else
                {
                    return PartialView("_Crud", new CreateArrearDemandModel());
                }
            }
        }


        // POST: ArrearsDemands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Description,Date")] ArrearsDemand arrearsDemand)
        {
            if (ModelState.IsValid)
            {
                arrearsDemand.PDUId = _sessionHelper.GetUserPDUId();
                _context.Add(arrearsDemand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(arrearsDemand);
        }
        public async Task<JsonResult> Save(CreateArrearDemandModel vM)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                vM.PDUId = _sessionHelper.GetUserPDUId();
                var r = _mapper.Map<ArrearsDemand>(vM);
                var (IsSaved, Message) = vM.Id == 0 ? await _arrearsDemand.Insert(r) : await _arrearsDemand.Update(r);
                helper.RCode = IsSaved ? 1 : 0;
                helper.RText = Message;
            }
            else
            {
                helper.RCode = 0;
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var item in allErrors)
                {
                    helper.RText += item.ErrorMessage;
                }
            }
            return Json(helper);
        }
        public async Task<IActionResult> Load()
        {
            var model = new ArrearsDemandViewModel
            {
                ArrearsDemands = await _arrearsDemand.GetAll(_sessionHelper.GetUserPDUId()),
                ArrearsPayments = await _arrearsPayment.GetAll(_sessionHelper.GetUserPDUId())

            };
            return PartialView("_List", model);
        }

        // GET: ArrearsDemands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cContext = _context.ArrearsDemands;
            if (cContext == null)
                return NotFound();
            var arrearsDemand = await cContext.FirstOrDefaultAsync(m => m.Id == id);
            if (arrearsDemand == null)
            {
                return NotFound();
            }

            return View(arrearsDemand);
        }

        // POST: ArrearsDemands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cContext = _context.ArrearsDemands;
            if (cContext == null)
                return NotFound();
            var arrearsDemand = await cContext.FindAsync(id);
            if (arrearsDemand == null)
                return NotFound();
            cContext.Remove(arrearsDemand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArrearsDemandExists(int id)
        {
            var list = _context.ArrearsDemands;
            if (list != null)
            {
                return list.Any(e => e.Id == id);
            }
            return false;
        }

        [HttpPost]
        public async Task<bool> MarkItPaid(int Id = 0)
        {
            if (Id == 0)
                return false;
            return await _arrearsDemand.MarkItPay(Id);
        }
    }
}