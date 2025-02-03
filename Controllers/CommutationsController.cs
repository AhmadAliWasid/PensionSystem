using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using PensionSystem.Entities.DTOs;
using System.Net.Http;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class CommutationsController(ApplicationDbContext context, IPensioner pensioner, ICheque cheque, SessionHelper sessionHelper, ICommutation commutation, HttpClient httpClient, IHttpClientFactory httpClientFactory) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IPensioner _pensioner = pensioner;
        private readonly ICheque _cheque = cheque;
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly ICommutation _commutation = commutation;
        private readonly HttpClient _apiClient = httpClient;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        // GET: Commutations
        public async Task<IActionResult> Index()
        {
            var commutations = _context.Commutations;
            return commutations != null ? View(await commutations.Include(x => x.Pensioner)
            .OrderByDescending(c => c.Month).ThenBy(p => p.Pensioner.PPOSystem).ToListAsync()) : View();
        }
        [HttpGet]
        public async Task<IActionResult> Crud(int id)
        {

            if (id == 0)
            {
                ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(3, _sessionHelper.GetUserPDUId()), "Value", "Text");
                ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
                var r = new UpdateCommutationDTO()
                {
                    Month = DateTime.Now
                };
                return PartialView("_Crud", r);
            }
            else
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    client.BaseAddress = _sessionHelper.GetUri();
                    var r = await client.GetFromJsonAsync<UpdateCommutationDTO>($"/api/Commutations/{id}");
                    if (r == null)
                        return NotFound();
                    ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(3, _sessionHelper.GetUserPDUId()), "Value", "Text", r.ChequeId);
                    ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text", r.PensionerId);
                    return PartialView("_Crud", r);

                }
                catch (Exception exc)
                {
                    return BadRequest(exc.Message);
                }
            }
        }
        public async Task<JsonResult> Save(Commutation vM)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                if (vM.Id == 0)
                {
                    var r = new Commutation()
                    {
                        Amount = vM.Amount,
                        ChequeId = vM.ChequeId,
                        Month = vM.Month,
                        PensionerId = vM.PensionerId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    var response = await _commutation.Insert(r);
                    if (response.IsSaved)
                    {
                        helper.RCode = 1;
                    }
                    else
                    {
                        helper.RCode = 0;
                        helper.RText = response.Message;
                    }
                }
                else
                {
                    var response = await _commutation.Update(vM);
                    if (response.IsSaved)
                    {
                        helper.RCode = 1;
                    }
                    else
                    {
                        helper.RCode = 0;
                        helper.RText = response.Message;
                    }
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
        // GET: Commutations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(3, _sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            return View();
        }

        // POST: Commutations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PensionerId,Month,Amount,CreatedDate,ModifiedDate,ChequeId")] Commutation commutation)
        {
            ModelState.Remove("Pensioner");
            if (ModelState.IsValid)
            {
                _context.Add(commutation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChequeId"] = new SelectList(await _cheque.GetOptions(3, _sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _pensioner.GetOptions(_sessionHelper.GetUserPDUId(), true), "Value", "Text");
            return View(commutation);
        }

        // GET: Commutations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commutation = await _context.Commutations.FindAsync(id);
            if (commutation == null)
            {
                return NotFound();
            }
            return View(commutation);
        }

        // POST: Commutations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PensionerId,Month,Amount,CreatedDate,ModifiedDate")] Commutation commutation)
        {
            if (id != commutation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(commutation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommutationExists(commutation.Id))
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
            return View(commutation);
        }

        // GET: Commutations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commutation = await _context.Commutations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (commutation == null)
            {
                return NotFound();
            }

            return View(commutation);
        }

        // POST: Commutations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var commutation = await _context.Commutations.FindAsync(id);
            _context.Commutations.Remove(commutation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommutationExists(int id)
        {
            return _context.Commutations.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> Load()
        {
            try
            {
                var list = new List<CommutationDTO>();
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(_sessionHelper.GetUri() + "api/Commutations/GetByPDUId(" + _sessionHelper.GetUserPDUId() + ")");
                response.EnsureSuccessStatusCode();
                list.AddRange(await response.Content.ReadFromJsonAsync<IEnumerable<CommutationDTO>>());
                return PartialView("_list", list);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);

            }
        }
    }
}