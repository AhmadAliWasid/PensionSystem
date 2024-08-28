using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminAuthorizeController : Controller
    {
    }
}