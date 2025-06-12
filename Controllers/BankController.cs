using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;
using WebAPI.Helpers;

namespace PensionSystem.Controllers
{
    public class BankController(IBank bank) : AdminAuthorizeController
    {
        private readonly IBank _bank = bank;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                return View("_Crud", new BankVM());
            }
            else
            {
                BankVM bankVM = new();
                var bank = await _bank.GetById(id);
                if (bank != null)
                {
                    bankVM.Id = bank.Id;
                    bankVM.Name = bank.Name;
                    bankVM.ShortName = bank.ShortName;
                    bankVM.Limit = bank.Limit;
                }

                return View("_Crud", bankVM);
            }
        }

        public async Task<JsonResult> Save(BankVM vM)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                var response = await _bank.Save(vM);
                if (response.IsSaved)
                {
                    helper.RCode = 1;
                }
                else
                {
                    helper.RCode = 0;
                    helper.RText = response.Message;
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
            return PartialView("_List", await _bank.GetAll());
        }

        public async Task<bool> Delete(int id)
        {
            return await _bank.Delete(id);
        }
    }
}