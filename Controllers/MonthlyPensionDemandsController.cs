using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using System.Text;
using System.Text.Json;
using PensionSystem.Entities.DTOs;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class MonthlyPensionDemandsController(ApplicationDbContext context, IMonthlyPensionDemand demand, IHBLPayments hblPayments,
        IPensionPayment pensionPayment, ICommutation commutation, IHBLArrears hBLArrears, SessionHelper sessionHelper,
        IHttpClientFactory httpClientFactory
        , HttpClient httpClient) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMonthlyPensionDemand _mpDemand = demand;
        private readonly IPensionPayment _pensionPayment = pensionPayment;
        private readonly IHBLPayments _hblPayments = hblPayments;
        private readonly ICommutation _commutation = commutation;
        private readonly IHBLArrears _hBLArrears = hBLArrears;
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly HttpClient _apiClient = httpClient;
        // GET: MonthlyPensionDemands
        public async Task<IActionResult> Index()
        {
            var vm = new MonthlyPensionDemandVM
            {
                PensionerPayments = await _context.PensionerPayments.ToListAsync(),
                monthlyPensionDemands = await _mpDemand.GetAll(_sessionHelper.GetUserPDUId())
            };
            return View(vm);
        }

        // GET: MonthlyPensionDemands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monthlyPensionDemand = await _context.MonthlyPensionDemands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monthlyPensionDemand == null)
            {
                return NotFound();
            }

            return View(monthlyPensionDemand);
        }

        // GET: MonthlyPensionDemands/Create
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateMPDemandDTO mPDemandDTO)
        {
            if (ModelState.IsValid)
            {
                mPDemandDTO.PDUId = _sessionHelper.GetUserPDUId();
                var httpRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_sessionHelper.GetUri()}api/MPDemand"),
                    Content = new StringContent(JsonSerializer.Serialize(mPDemandDTO), Encoding.UTF8, "application/json")
                };
                var response = await _apiClient.SendAsync(httpRequest);
                var r = await response.Content.ReadFromJsonAsync<CreateMPDemandDTO>();
                return r != null ? Ok(r) : BadRequest(r);
            }
            else
            {
                ModelState.AddModelError("error", "Invalid Data");
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = _sessionHelper.GetUri();
                var response = await client.GetFromJsonAsync<UpdateMPDemandDTO>($"/api/MPDemand/{id}");
                return View(response);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        // POST: MonthlyPensionDemands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Description,Number,Date")] MonthlyPensionDemand monthlyPensionDemand)
        {
            if (id != monthlyPensionDemand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    monthlyPensionDemand.PDUId = _sessionHelper.GetUserPDUId();
                    _context.Update(monthlyPensionDemand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonthlyPensionDemandExists(monthlyPensionDemand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(monthlyPensionDemand);
        }

        // GET: MonthlyPensionDemands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monthlyPensionDemand = await _context.MonthlyPensionDemands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monthlyPensionDemand == null)
            {
                return NotFound();
            }

            return View(monthlyPensionDemand);
        }

        // POST: MonthlyPensionDemands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monthlyPensionDemand = await _context.MonthlyPensionDemands.FindAsync(id);
            if (monthlyPensionDemand == null) { return NotFound(); }
            _context.MonthlyPensionDemands.Remove(monthlyPensionDemand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonthlyPensionDemandExists(int id)
        {
            return _context.MonthlyPensionDemands.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            MPDVMPrint vm = new();
            var demand = await _mpDemand.Get(id);
            if (demand == null) { return NotFound(); }
            var lastDate = UserFormat.GetLastMonth(demand.Date);
            var lastDateForHBL = new DateOnly(lastDate.Year, lastDate.Month, lastDate.Day);
            vm.MonthlyPensionDemand = demand;
            vm.PensionerPayments = await _pensionPayment.GetPensionPayments(id);
            vm.HBLPayments = await _hblPayments.GetByMonth(lastDate, _sessionHelper.GetUserPDUId());
            vm.Commutations = await _commutation.GetCommutationsByMonth(lastDateForHBL, _sessionHelper.GetUserPDUId());
            vm.HBLArrears = await _hBLArrears.GetArrearsByMonth(lastDate, _sessionHelper.GetUserPDUId());
            return PartialView("_Print", vm);
        }

        [HttpGet]
        public async Task<ActionResult> GetDemandOptions(DateTime month)
        {
            return Json(
                await _context.MonthlyPensionDemands
                .Where(d =>
                d.Date.Month == month.Month
                && d.Date.Year == month.Year
                && d.PDUId == _sessionHelper.GetUserPDUId())
                .Select(x => new
                {
                    Value = x.Id,
                    Text = x.Number + "-" + x.Date.ToString("dd-MM-yyyy")
                }).ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Load()
        {
            // getting all regions from web api
            var list = new List<MPDemandDTO>();
            try
            {
                var client = _httpClientFactory.CreateClient();
                string uri = _sessionHelper.GetUri() + "/api/MPDemand";
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                list.AddRange(await response.Content.ReadFromJsonAsync<IEnumerable<MPDemandDTO>>());
                return PartialView("_list", list);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);

            }
        }
    }
}