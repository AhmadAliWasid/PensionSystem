using Microsoft.AspNetCore.Authorization;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser")]
    public class PDUAuthorizeController : AdminAuthorizeController
    {
    }
}