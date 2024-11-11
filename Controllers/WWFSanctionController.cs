using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Pension.Entities.Helpers;
using PensionSystem.Entities.DTOs;
using PensionSystem.Helpers;
using PensionSystem.ViewModels;
using System.Text;
using WebAPI.Interfaces;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class WWFSanctionController(SessionHelper sessionHelper,
        IMapper mapper, HttpClient httpClient, IHttpClientFactory httpClientFactory, IWWFSanction wWFSanction) : Controller
    {
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly IMapper _mapper = mapper;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IWWFSanction _wWFSanction = wWFSanction;

        // GET: ArrearsDemands
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                var r = new WWFSanctionDTO
                {
                    Date = DateTime.Now
                };
                return PartialView("_Crud", r);
            }
            else
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = _sessionHelper.GetUri();
                var r = await client.GetFromJsonAsync<WWFSanctionDTO>($"api/WWFSanction/{id}");
                if (r != null)
                {
                    return PartialView("_Crud", r);
                }
                else
                {
                    return PartialView("_Crud", new WWFSanctionDTO());
                }
            }
        }

        public async Task<IActionResult> Save(WWFSanctionDTO entity)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                entity.PDUId = _sessionHelper.GetUserPDUId();
                if (entity.Id == 0)
                {
                    // add 
                    var record = _mapper.Map<CreateWWFSanctionDTO>(entity);
                    var httpRequest = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri($"{_sessionHelper.GetUri()}api/WWFSanction"),
                        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(record), Encoding.UTF8, "application/json")
                    };
                    var response = await _httpClient.SendAsync(httpRequest);
                    var r = await response.Content.ReadFromJsonAsync<WWFSanctionDTO>();
                    return r != null ? Ok(r) : BadRequest(r);
                }
                else
                {
                    var httpRequest = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri($"{_sessionHelper.GetUri()}api/WWFSanction/{entity.Id}"),
                        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json")
                    };
                    var response = await _httpClient.SendAsync(httpRequest);
                    if (response.IsSuccessStatusCode)
                        return Ok();
                    else
                        return BadRequest();
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
            var response = await _httpClient.GetAsync($"{_sessionHelper.GetUri()}api/WWFSanction/GetByPDUId({_sessionHelper.GetUserPDUId()})");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<WWFSanctionDTO>>(jsonString);

                return PartialView("_List", list);
            }
            else
            {
                // Handle error
                return StatusCode((int)response.StatusCode, "Unable to retrieve data from API.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_sessionHelper.GetUri()}api/WWFSanction/{id}")
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
    }
}