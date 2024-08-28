using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pension.Entities.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Controllers
{
    public class PDUController : AdminAuthorizeController
    {
        private readonly IPDU _pdu;

        public PDUController(IPDU pdu)
        {
            _pdu = pdu;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                return View("_Crud", new PDUVM());
            }
            else
            {
                PDUVM bankVM = new();
                var record = await _pdu.Get(id);
                if (record != null)
                {
                    bankVM.Id = record.Id;
                    bankVM.Name = record.Name;
                    bankVM.ShortName = record.ShortName;
                    bankVM.AMStamp = record.AMStamp;
                    bankVM.DMStamp = record.DMStamp;
                    bankVM.BaseStamp = record.BaseStamp;
                }
                return View("_Crud", bankVM);
            }
        }

        public async Task<JsonResult> Save(PDUVM vM)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                PDU pDU = new PDU()
                {
                    Id = vM.Id,
                    Name = vM.Name,
                    ShortName = vM.ShortName,
                    AMStamp = vM.AMStamp,
                    DMStamp = vM.DMStamp,
                    BaseStamp = vM.BaseStamp
                };
                var response = await _pdu.Save(pDU);
                if (response.isSaved)
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
            return PartialView("_List", await _pdu.GetAll());
        }
    }
}