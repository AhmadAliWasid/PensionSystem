using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ProformaController : Controller
    {
        private readonly IPensioner _pensioner;
        private readonly SessionHelper _sessionHelper;

        public ProformaController(IPensioner pensioner, SessionHelper sessionHelper)
        {
            _pensioner = pensioner;
            _sessionHelper = sessionHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _pensioner.GetActive(_sessionHelper.GetUserPDUId()));
        }

        [HttpGet]
        public async Task<IActionResult> Print(int id, bool blank = false)
        {
            var pensioner = await _pensioner.Get(id);
            if (pensioner == null)
            {
                return NotFound();
            }

            return PartialView("_Print", pensioner);
        }

        public async Task<IActionResult> PageSlip(int id)
        {
            var pensioner = await _pensioner.Get(id);
            if (pensioner == null)
            {
                return NotFound();
            }
            return View(pensioner);
        }

        public async Task<IActionResult> Page(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pensioner = await _pensioner.Get(id.Value);
            if (pensioner == null)
            {
                return NotFound();
            }
            return PartialView("_Page", pensioner);
        }

        [HttpGet]
        public async Task<IActionResult> GetAliveCertificate(int id)
        {
            var pensioner = await _pensioner.Get(id);
            if (pensioner == null)
            {
                return NotFound();
            }
            return PartialView("_LifeCertificate", pensioner);
        }

        [HttpGet]
        public async Task<IActionResult> GetACO(int id)
        {
            var pensioner = await _pensioner.Get(id);
            if (pensioner == null)
            {
                return NotFound();
            }
            return PartialView("_AccountOpening", pensioner);
        }
    }
}