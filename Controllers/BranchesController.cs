using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pension.Entities.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;

namespace PensionSystem.Controllers
{
    public class BranchesController : Controller
    {
        private readonly IBranch _branch;
        private readonly IBank _bank;

        public BranchesController(IBranch branch, IBank bank)
        {
            _branch = branch;
            _bank = bank;
        }

        // GET: Branches
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text");
                return View("_Crud", new BranchVM());
            }
            else
            {
                var branch = await _branch.GetVM(id);
                ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text", branch.BankId);
                return View("_Crud", branch);
            }
        }

        public async Task<JsonResult> Save(BranchVM vM)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                var (IsSaved, Message) = await _branch.Save(vM);
                if (IsSaved)
                {
                    helper.RCode = 1;
                }
                else
                {
                    helper.RCode = 0;
                    helper.RText = Message;
                }
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
            return PartialView("_List", await _branch.GetAll());
        }

        [HttpGet]
        public async Task<JsonResult> GetOptions(int bankId)
        {
            return Json(await _branch.GetOptions(bankId));
        }
    }
}