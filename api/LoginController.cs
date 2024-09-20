using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Entities.DTOs;
using System;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(UserManager<IdentityUser> userManager,
        ILogger<LoginModel> logger,
        SignInManager<IdentityUser> signInManager) : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ILogger<LoginModel> _logger = logger;
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginModel)
        {

            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(loginModel.Email,
                loginModel.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.Users.Where(x => x.Email == loginModel.Email)
                    .FirstOrDefaultAsync();
                if (user != null)
                {

                }
                return Ok();
            }
            return BadRequest();
        }
    }

}
