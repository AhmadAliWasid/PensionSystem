using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;

namespace PensionSystem.Controllers
{
    public class RoleController : AdminAuthorizeController
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Crud(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("_Crud");
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(id);
                return View("_Crud", role);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Save(IdentityRole identityRole)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                try
                {
                    if (identityRole.Id == "0")
                    {
                        identityRole.Id = Guid.NewGuid().ToString();
                        await _roleManager.CreateAsync(identityRole);
                    }
                    else
                    {
                        await _roleManager.UpdateAsync(identityRole);
                    }
                    helper.RCode = 1;
                }
                catch (Exception exc)
                {
                    helper.RCode = 0;
                    helper.RText = exc.Message;
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

        [HttpGet]
        public async Task<IActionResult> Load()
        {
            return PartialView("_List", await _roleManager.Roles.ToListAsync());
        }
    }
}