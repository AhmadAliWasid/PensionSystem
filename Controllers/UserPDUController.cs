using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using WebAPI.Helpers;

namespace PensionSystem.Controllers
{
    public class UserPDUController : AdminAuthorizeController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserPDU _userPDU;
        private readonly IPDU _pdu;

        public UserPDUController(UserManager<IdentityUser> userManager, IUserPDU userPDU, IPDU pdu)
        {
            _userManager = userManager;
            _userPDU = userPDU;
            _pdu = pdu;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Crud()
        {
            ViewData["PDUId"] = new SelectList(await _pdu.GetOptions(), "Value", "Text");
            ViewData["UserId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
            return View("_Crud");
        }

        [HttpPost]
        public async Task<JsonResult> Save(UserPDUVM userPDUVM)
        {
            JsonResponseHelper helper = new();
            try
            {
                UserPDU userPDU = new()
                {
                    PDUId = userPDUVM.PDUId,
                    UserId = userPDUVM.UserId
                };
                var result = await _userPDU.Save(userPDU);
                if (result.IsSaved)
                {
                    helper.RCode = 1;
                }
                return Json(helper);
            }
            catch (Exception exc)
            {
                helper.RCode = 0;
                helper.RText = exc.Message;
            }

            return Json(helper);
        }

        [HttpGet]
        public async Task<IActionResult> Load()
        {
            return PartialView("_List", await _userPDU.GetAll());
        }
    }
}