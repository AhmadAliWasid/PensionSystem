using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PensionSystem.ViewModels;
using WebAPI.Helpers;

namespace PensionSystem.Controllers
{
    public class UserRoleController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Crud()
        {
            ViewData["RoleId"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name");
            ViewData["UserId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
            return View("_Crud");
        }

        [HttpPost]
        public async Task<JsonResult> Save(string UserId, string RoleId)
        {
            JsonResponseHelper helper = new();
            try
            {
                if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(RoleId))
                {
                    return Json(helper);
                }
                else
                {
                    var users = await _userManager.Users.ToListAsync();
                    var selectedUser = users.Where(x => x.Id == UserId).FirstOrDefault();
                    var roles = await _roleManager.Roles.ToListAsync();
                    var selectedRole = roles.Where(x => x.Id == RoleId).FirstOrDefault();
                    if (selectedRole != null && selectedUser != null)
                    {
                        await _userManager.AddToRoleAsync(selectedUser, selectedRole.Name);
                        helper.RCode = 1;
                        return Json(helper);
                    }
                }
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
            var users = await _userManager.Users.ToListAsync();
            List<UserRoleVM> roles = new List<UserRoleVM>();
            foreach (var user in users)
            {
                var rolesAssigned = await _userManager.GetRolesAsync(user);
                foreach (var role in rolesAssigned)
                {
                    roles.Add(new UserRoleVM()
                    {
                        Email = user.Email,
                        Username = user.UserName,
                        Role = role,
                        UserId = user.Id
                    });
                }
            }
            return PartialView("_List", roles);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var role = await _roleManager.FindByNameAsync(roleName);
            if (user == null)
                return BadRequest("User Not Found!");
            if (role == null)
                return BadRequest("Role Not Found");
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}