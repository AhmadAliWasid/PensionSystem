using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Helpers;
using PensionSystem.Interfaces;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ProformaController(IPensioner pensioner, SessionHelper sessionHelper, IMapper mapper) : Controller
    {
        private readonly IPensioner _pensioner = pensioner;
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly IMapper _mapper = mapper;
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pduId = _sessionHelper.GetUserPDUId();
            var pensioners = await _pensioner.GetActive(pduId);
            return View(pensioners);
        }

        [HttpGet]
        public async Task<IActionResult> Print([FromQuery] int id, [FromQuery] bool blank = false)
        {
            return await GetPensionerPartialViewAsync(id, "_Print");
        }

        [HttpGet]
        public async Task<IActionResult> PageSlip([FromRoute] int id)
        {
            return await GetPensionerViewAsync(id, "PageSlip");
        }

        [HttpGet]
        public async Task<IActionResult> Page([FromQuery] int? id)
        {
            if (id == null)
                return NotFound();
            return await GetPensionerPartialViewAsync(id.Value, "_Page");
        }

        [HttpGet]
        public async Task<IActionResult> GetAliveCertificate([FromQuery] int id)
        {
            return await GetPensionerPartialViewAsync(id, "_LifeCertificate");
        }

        [HttpGet]
        public async Task<IActionResult> GetLPC([FromQuery] int id)
        {
            return await GetPensionerPartialViewAsync(id, "_LPC");
        }
        [HttpGet]
        public async Task<IActionResult> GetPPC([FromQuery] int id)
        {
            return await GetPensionerPartialViewAsync(id, "_PPC");
        }
        [HttpGet]
        public async Task<IActionResult> GetACO([FromQuery] int id)
        {
            return await GetPensionerPartialViewAsync(id, "_AccountOpening");
        }
        [HttpGet]
        public async Task<IActionResult> GetFreeSupply([FromQuery] int id)
        {
            return await GetPensionerPartialViewAsync(id, "_FreeSupply");
        }
        // --- Private helpers ---

        private async Task<IActionResult> GetPensionerPartialViewAsync(int id, string partialViewName)
        {
            var pensioner = await _pensioner.Get(id);
            var r = _mapper.Map<PensionerDTO>(pensioner);
            r.Session = new SessionVM
            {
                DMStamp = _sessionHelper.GetDMStamp(),
                AMStamp = _sessionHelper.GetAMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };

            if (pensioner == null)
                return NotFound();
            return PartialView(partialViewName, r);
        }

        private async Task<IActionResult> GetPensionerViewAsync(int id, string viewName)
        {
            var pensioner = await _pensioner.Get(id);
            var r = _mapper.Map<PensionerDTO>(pensioner);
               ;
            r.Session = new SessionVM
            {
                DMStamp = _sessionHelper.GetDMStamp(),
                AMStamp = _sessionHelper.GetAMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };
            if (pensioner == null)
                return NotFound();
            return View(viewName, r);
        }
    }
}