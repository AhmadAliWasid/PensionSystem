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
using Azure;

namespace WebAPI.Controllers
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
        public IActionResult Index()
        {
            return View();
        }


        // GET: MonthlyPensionDemands/Create
        [HttpGet]
        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                return PartialView("_Crud", new MPDemandDTO() { Date = DateTime.Now });
            }
            else
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    client.BaseAddress = _sessionHelper.GetUri();
                    return PartialView("_Crud", await client.GetFromJsonAsync<UpdateMPDemandDTO>($"/api/MPDemand/{id}"));

                }
                catch (Exception exc)
                {
                    return BadRequest(exc.Message);
                }

            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Save(MPDemandDTO mPDemandDTO)
        {
            var res = new JsonResponseHelper();
            if (ModelState.IsValid)
            {
                if (mPDemandDTO.Id == 0)
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
                    if (r == null)
                        res.RCode = 0;
                    else
                        res.RCode = 1;
                }
                else
                {
                    mPDemandDTO.PDUId = _sessionHelper.GetUserPDUId();
                    var httpRequest = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri($"{_sessionHelper.GetUri()}api/MPDemand/{mPDemandDTO.Id}"),
                        Content = new StringContent(JsonSerializer.Serialize(mPDemandDTO), Encoding.UTF8, "application/json")
                    };
                    var response = await _apiClient.SendAsync(httpRequest);
                    var r = await response.Content.ReadFromJsonAsync<CreateMPDemandDTO>();
                    if (r == null)
                        res.RCode = 0;
                    else
                        res.RCode = 1;


                }
            }
            else
            {
                ModelState.AddModelError("error", "Invalid Data");

            }
            return Json(res);
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
            // getting i
            try
            {
                var list = new List<MPDemandDTO>();
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(_sessionHelper.GetUri() + "api/MPDemand/GetByPDUId(" + _sessionHelper.GetUserPDUId() + ")");
                response.EnsureSuccessStatusCode();
                list.AddRange(await response.Content.ReadFromJsonAsync<IEnumerable<MPDemandDTO>>());
                var vm = new MonthlyPensionDemandVM
                {
                    PensionerPayments = await _context.PensionerPayments.ToListAsync(),
                    MonthlyPensionDemands = list
                };
                return PartialView("_list", vm);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);

            }
        }
    }
}