using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PensionSystem.Entities.DTOs;
using PensionSystem.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(UserManager<IdentityUser> userManager,
        ILogger<LoginModel> logger,
        SignInManager<IdentityUser> signInManager, IConfiguration configuration, IUserPDU userPDU) : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<LoginModel> _logger = logger;
        private readonly IUserPDU _userPDU = userPDU;

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginModel)
        {

            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(loginModel.Email,
                loginModel.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.Users.Where(x => x.Email == loginModel.Email)
                    .FirstOrDefaultAsync();
                // creating token after login
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
        {
                new Claim(ClaimTypes.Email , user.Email)
            };
                var PDU = await _userPDU.GetByUserId(user.Id);
                if (PDU == null)
                    return BadRequest();
                var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddDays(60),
                        signingCredentials: credentials);
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Accepted(new LoggedInDTO()
                {
                    AccessToken = jwt,
                    TokenType = "beared",
                    UserId = user.Id.ToString(),
                    UserName = user.UserName,
                    PDUId = PDU.Id.ToString(),
                });
            }
            return BadRequest();
        }
    }

}
