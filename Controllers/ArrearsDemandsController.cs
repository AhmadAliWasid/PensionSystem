using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Entities.DTOs;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;
using System.Text;
using System.Text.Json;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ArrearsDemandsController(ApplicationDbContext context, IArrearsDemand arrearsDemand, SessionHelper sessionHelper,
        IMapper mapper, IArrearsPayment arrearsPayment, HttpClient httpClient, IHttpClientFactory httpClientFactory) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IArrearsDemand _arrearsDemand = arrearsDemand;
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly IMapper _mapper = mapper;
        private readonly IArrearsPayment _arrearsPayment = arrearsPayment;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        // GET: ArrearsDemands
        public async Task<IActionResult> Index()
        {
            ArrearsDemandViewModel model = new();
            var arrearsContext = _context.ArrearsPayments;
            if (arrearsContext != null)
            {
                model.ArrearsDemands = await _arrearsDemand.GetArrears(_sessionHelper.GetUserPDUId());
                model.ArrearsPayments = await arrearsContext.ToListAsync();
            }
            return View(model);
        }
        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                var r = new ArreardDemandDTO
                {
                    Date = DateTime.Now
                };
                return PartialView("_Crud", r);
            }
            else
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = _sessionHelper.GetUri();
                var r = await client.GetFromJsonAsync<ArreardDemandDTO>($"api/ArrearDemand/{id}");
                if (r != null)
                {
                    return PartialView("_Crud", r);
                }
                else
                {
                    return PartialView("_Crud", new ArreardDemandDTO());
                }
            }
        }

        public async Task<IActionResult> Save(ArreardDemandDTO entity)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                entity.PDUId = _sessionHelper.GetUserPDUId();
                if (entity.Id == 0)
                {
                    // add 
                    var record = _mapper.Map<CreateArreardDemandDTO>(entity);
                    var httpRequest = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri($"{_sessionHelper.GetUri()}api/ArrearDemand"),
                        Content = new StringContent(JsonSerializer.Serialize(record), Encoding.UTF8, "application/json")
                    };
                    var response = await _httpClient.SendAsync(httpRequest);
                    var r = await response.Content.ReadFromJsonAsync<ArreardDemandDTO>();
                    return r != null ? Ok(r) : BadRequest(r);
                }
                else
                {
                    // update
                    var record = _mapper.Map<UpdateArreardDemandDTO>(entity);
                    var httpRequest = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri($"{_sessionHelper.GetUri()}api/ArrearDemand/{entity.Id}"),
                        Content = new StringContent(JsonSerializer.Serialize(record), Encoding.UTF8, "application/json")
                    };
                    var response = await _httpClient.SendAsync(httpRequest);
                    var r = await response.Content.ReadFromJsonAsync<ArreardDemandDTO>();
                    return r != null ? Ok(r) : BadRequest(r);
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
            return Ok(helper);
        }
        public async Task<IActionResult> Load()
        {
            var model = new ArrearsDemandViewModel
            {
                ArrearsDemands = await _arrearsDemand.GetAll(_sessionHelper.GetUserPDUId()),
                ArrearsPayments = await _arrearsPayment.GetAll(_sessionHelper.GetUserPDUId())

            };
            return PartialView("_List", model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_sessionHelper.GetUri()}api/ArrearDemand/{id}")
            };

            var response = await _httpClient.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                return Ok("Successfully Deleted");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return BadRequest(errorMessage);
            }
        }

        private bool ArrearsDemandExists(int id)
        {
            var list = _context.ArrearsDemands;
            if (list != null)
            {
                return list.Any(e => e.Id == id);
            }
            return false;
        }

        [HttpPost]
        public async Task<bool> MarkItPaid(int Id = 0)
        {
            if (Id == 0)
                return false;
            return await _arrearsDemand.MarkItPay(Id);
        }
    }
}