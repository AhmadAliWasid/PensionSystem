using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.DTOs;
using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;
using System.Text;
using System.Text.Json;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "PDUUser")]
    public class PensionersController(
        ApplicationDbContext context,
        IRelation relation,
        ICompany company,
        IPensioner pensioner,
        IWebHostEnvironment webHostEnvironment,
        IBank bank,
        IBranch branch,
        IPDU pDU,
        SessionHelper sessionHelper,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICompany _company = company;
        private readonly IPensioner _pensioner = pensioner;
        private readonly IBank _bank = bank;
        private readonly IBranch _branch = branch;
        private readonly IPDU _pDU = pDU;
        private readonly IWebHostEnvironment _hostingEnvironment = webHostEnvironment;
        private readonly IRelation _relation = relation;
        private readonly SessionHelper _sessionHelper = sessionHelper;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Pensioners main view.
        /// </summary>
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Files() => PartialView("_Files");

        [HttpGet]
        public async Task<IActionResult> Increase() =>
            View(await _pensioner.GetActive(_sessionHelper.GetUserPDUId()));

        [HttpGet]
        public IActionResult BulkAddition() => View();

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var vm = new PensionerViewModel
            {
                Pensioners = await _pensioner.GetActive(_sessionHelper.GetUserPDUId()),
                Branches = await _branch.GetAll()
            };
            return PartialView("_list", vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vm = new PensionerViewModel
            {
                Pensioners = await _pensioner.GetInActiveList(_sessionHelper.GetUserPDUId()),
                Branches = await _branch.GetAll()
            };
            return PartialView("_list", vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetPensionerPhysicalVerification()
        {
            var vm = new PhysicalVerificationVM
            {
                Pensioner = await _pensioner.GetPhysicalVerification(_sessionHelper.GetUserPDUId())
            };
            return PartialView("_PhysicalVerification", vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetSMSList() =>
            PartialView("_SMSService", await _pensioner.GetSMSList(_sessionHelper.GetUserPDUId()));

        [HttpGet]
        public async Task<IActionResult> CreateUpdate(int? id)
        {
            PensionerDTO dto = new();
            if (id == null || id == 0)
            {
                await PopulateSelectListsAsync();

                return PartialView("_CreateUpdate", dto);
            }

            var pensioner = await _context.Pensioner
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pensioner == null)
                return NotFound();

            MapPensionerToDTO(pensioner, dto);
            await PopulateSelectListsAsync(pensioner.CompanyId, pensioner.RelationId, pensioner.Branch?.BankId, pensioner.BranchId);

            return PartialView("_CreateUpdate", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(PensionerDTO dto)
        {
            var response = new JsonResponseHelper();

            if (!ModelState.IsValid)
            {
                response.RText = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(response);
            }

            if (dto.Id == 0)
            {
                var entity = MapDTOToPensioner(dto);
                entity.PDUId = _sessionHelper.GetUserPDUId();
                await _context.Pensioner.AddAsync(entity);
            }
            else
            {
                var entity = await _context.Pensioner.FindAsync(dto.Id);
                if (entity == null)
                {
                    response.RCode = 0;
                    response.RText = "Unable to find in DB";
                    return Json(response);
                }
                MapDTOToPensioner(dto, entity);
                _context.Pensioner.Update(entity);
            }

            try
            {
                await _context.SaveChangesAsync();
                response.RCode = 1;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                response.RText = ExceptionHelper.GetDetail(ex);
            }

            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var pensioner = await _context.Pensioner
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pensioner == null)
                return NotFound();

            return View(pensioner);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pensioner = await _context.Pensioner.FindAsync(id);
            if (pensioner == null)
                return View();

            _context.Pensioner.Remove(pensioner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetRestored() =>
            PartialView("_Restored", await _pensioner.GetRestored(_sessionHelper.GetUserPDUId()));

        [HttpGet, HttpPost]
        public async Task<bool> RestoreNow(int pensionerId) =>
            await _pensioner.RestoreNow(pensionerId);

        [HttpPost]
        public async Task<JsonResult> GetMaxPageNumber(int companyId)
        {
            int maxNumber = await _pensioner.GetMaxPageNumber(companyId) + 1;
            return Json(new { MaxNumber = maxNumber });
        }

        [HttpPost]
        public async Task<JsonResult> UploadFileOfRatesRevision(IFormFile file)
        {
            var result = new JsonResponseHelper();

            if (file == null || file.Length == 0)
            {
                result.RText = "File Not Selected";
                return Json(result);
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                result.RText = "Invalid file type";
                return Json(result);
            }

            var rootFolder = Path.Combine(_hostingEnvironment.WebRootPath, "files", "rateRevisionFolder");
            Directory.CreateDirectory(rootFolder);

            var filePath = Path.Combine(rootFolder, Path.GetFileName(file.FileName));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (file.Length <= 0)
            {
                result.RText = "File has no data";
                return Json(result);
            }

            try
            {
                using var reader = new StreamReader(filePath, Encoding.Default, false);
                int rowNumber = 1;
                string? currentLine;
                while ((currentLine = await reader.ReadLineAsync()) != null)
                {
                    if (rowNumber != 1)
                    {
                        var values = currentLine.Split(',');
                        if (values.Length < 5)
                        {
                            result.RText = $"Invalid data at row {rowNumber}";
                            return Json(result);
                        }

                        if (!int.TryParse(values[0], out int id) ||
                            !decimal.TryParse(values[1], out decimal mp) ||
                            !decimal.TryParse(values[2], out decimal cma) ||
                            !decimal.TryParse(values[3], out decimal orderly) ||
                            !decimal.TryParse(values[4], out decimal total))
                        {
                            result.RText = $"Invalid data at row {rowNumber}";
                            return Json(result);
                        }

                        var pensioner = await _context.Pensioner.FindAsync(id);
                        if (pensioner == null)
                        {
                            result.RText = $"{id} not found in DB at row {rowNumber}";
                            return Json(result);
                        }

                        pensioner.MonthlyPension = mp;
                        pensioner.CMA = cma;
                        pensioner.OrderelyAllowence = orderly;
                        pensioner.Total = total;
                        _context.Pensioner.Update(pensioner);
                    }
                    rowNumber++;
                }
                await _context.SaveChangesAsync();
                result.RText = "Ok";
                result.RCode = 1;
            }
            catch (Exception ex)
            {
                result.RText = ex.Message;
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<bool> UpdateSMSServiceStatus(int pensionerId, bool status) =>
            await _pensioner.UpdateSMSServiceStatus(pensionerId, status);

        [HttpPost]
        public async Task<JsonResult> UploadFileOfBulkAdditionOfPensioner(IFormFile file)
        {
            var result = new JsonResponseHelper();

            if (file == null || file.Length == 0)
            {
                result.RText = "File Not Selected";
                return Json(result);
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                result.RText = "Invalid file type";
                return Json(result);
            }

            var rootFolder = Path.Combine(_hostingEnvironment.WebRootPath, "files", "BulkAdditionOfPensionerFolder");
            Directory.CreateDirectory(rootFolder);

            var filePath = Path.Combine(rootFolder, Path.GetFileName(file.FileName));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (file.Length <= 0)
            {
                result.RText = "File has no data";
                return Json(result);
            }

            try
            {
                using var reader = new StreamReader(filePath, Encoding.Default, false);
                int rowNumber = 1;
                string? currentLine;
                var pensionUploadVM = new List<PensionerUploadVM>();
                while ((currentLine = await reader.ReadLineAsync()) != null)
                {
                    if (rowNumber != 1)
                    {
                        var values = currentLine.Split(',');
                        if (!ValidateRow(values, out int pduId, out int ppoNumber, out string name,
                            out string designation, out int bps, out string claimant,
                            out string claimantCnic, out string companyShortName, out string bankShortName,
                            out string branchCode, out string accountNumber, out decimal monthlyPension,
                            out decimal cma, out decimal orderly, out decimal total))
                        {
                            result.RText = $"Invalid data in PPO Number {ppoNumber}";
                            return Json(result);
                        }

                        if (name.Length > 19)
                        {
                            result.RText = $"PPO # {ppoNumber} Name must not exceed 19 Characters";
                            return Json(result);
                        }
                        if (designation.Length > 50)
                        {
                            result.RText = $"PPO # {ppoNumber} Designation must not exceed 50 Characters";
                            return Json(result);
                        }
                        if (accountNumber.Length > 19)
                        {
                            result.RText = $"PPO # {ppoNumber} Account Number must not exceed 19 Characters";
                            return Json(result);
                        }
                        if (claimant.Length > 50)
                        {
                            result.RText = $"PPO # {ppoNumber} Claimant must not exceed 50 Characters";
                            return Json(result);
                        }
                        if (claimantCnic.Length > 15)
                        {
                            result.RText = $"PPO # {ppoNumber} ClaimantCNIC must not exceed 15 Characters";
                            return Json(result);
                        }

                        pensionUploadVM.Add(new PensionerUploadVM
                        {
                            PDUId = pduId,
                            PPONumber = ppoNumber,
                            Name = name,
                            Designation = designation,
                            BPS = bps,
                            CMA = cma,
                            AccountNumber = accountNumber,
                            BankShortName = bankShortName,
                            BranchCode = branchCode,
                            Claimant = claimant,
                            ClaimantCNIC = claimantCnic,
                            CompanyShortName = companyShortName,
                            MonthlyPension = monthlyPension,
                            Orderly = orderly,
                            Total = total
                        });
                    }
                    rowNumber++;
                }

                var (isSaved, message) = await _pensioner.AddToDBFromFile(pensionUploadVM);
                result.RText = message;
                result.RCode = isSaved ? 1 : 0;
            }
            catch (Exception ex)
            {
                result.RText = ex.Message;
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(FileUploadDTO fileUploadDTO)
        {
            var client = _httpClientFactory.CreateClient();
            string baseUrl = _configuration["BaseUrl"];
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{baseUrl}/api/Pensioner/Upload"),
                Content = new StringContent(JsonSerializer.Serialize(fileUploadDTO), Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadFromJsonAsync<FileUploadDTO>();
                return Ok(r);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return BadRequest("Error: " + responseContent);
        }

        [HttpGet]
        public async Task<IActionResult> GetPensionerByPPO(int PPO)
        {
            var pensioner = await _pensioner.GetByPPO(PPO);
            if (pensioner == null)
                return NotFound("Pensioner not found.");
            return Ok(pensioner);
        }

        #region Helpers

        private async Task PopulateSelectListsAsync(int? companyId = null, int? relationId = null, int? bankId = null, int? branchId = null)
        {
            ViewData["CompanyId"] = new SelectList(await _company.GetOptions(), "Value", "Text", companyId);
            ViewData["RelationId"] = new SelectList(await _relation.GetOptions(), "Value", "Text", relationId);
            ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text", bankId);
            ViewData["BranchId"] = new SelectList(await _branch.GetOptions(), "Value", "Text", branchId);
        }

        private static void MapPensionerToDTO(Pensioner source, PensionerDTO dest)
        {
            dest.Id = source.Id;
            dest.PageNumber = source.PageNumber;
            dest.Name = source.Name;
            dest.Claimant = source.Claimant;
            dest.FatherName = source.FatherName;
            dest.Gender = source.Gender;
            dest.Spouse = source.Spouse;
            dest.Designation = source.Designation;
            dest.PPONumber = source.PPONumber;
            dest.PPOSystem = source.PPOSystem;
            dest.SanctionNumber = source.SanctionNumber;
            dest.SanctionDate = source.SanctionDate;
            dest.DOB = source.DOB;
            dest.DateOfRetirement = source.DateOfRetirement;
            dest.CompanyId = source.CompanyId;
            dest.Mobile = source.Mobile;
            dest.CNIC = source.CNIC;
            dest.ClaimantCNIC = source.ClaimantCNIC;
            dest.Address = source.Address;
            dest.IsActiveClaimant = source.IsActiveClaimant;
            dest.AccountNumber = source.AccountNumber;
            dest.MonthlyPension = source.MonthlyPension;
            dest.CMA = source.CMA;
            dest.OrderelyAllowence = source.OrderelyAllowence;
            dest.Total = source.Total;
            dest.Remarks = source.Remarks;
            dest.RelationId = source.RelationId;
            dest.BPS = source.BPS;
            dest.BranchId = source.BranchId;
            dest.DateOfAppointment = source.DateOfAppointment;
            dest.LastBasicPay = source.LastBasicPay;
            dest.AccountTitle = source.AccountTitle;
            dest.IsServiceActive = source.IsServiceActive;
            dest.RetiringOffice = source.RetiringOffice;
            dest.Commutation = source.Commutation;
            dest.MonthlyRecovery = source.MonthlyRecovery;
            dest.IBAN = source.IBAN;
        }

        private static Pensioner MapDTOToPensioner(PensionerDTO dto, Pensioner? entity = null)
        {
            entity ??= new Pensioner();
            entity.PageNumber = dto.PageNumber;
            entity.Name = dto.Name;
            entity.Claimant = dto.Claimant;
            entity.FatherName = dto.FatherName;
            entity.Gender = dto.Gender;
            entity.Spouse = dto.Spouse;
            entity.Designation = dto.Designation;
            entity.PPONumber = dto.PPONumber;
            entity.PPOSystem = dto.PPOSystem;
            entity.SanctionNumber = dto.SanctionNumber;
            entity.SanctionDate = dto.SanctionDate;
            entity.DOB = dto.DOB;
            entity.DateOfRetirement = dto.DateOfRetirement;
            entity.CompanyId = dto.CompanyId;
            entity.Mobile = dto.Mobile;
            entity.CNIC = dto.CNIC;
            entity.ClaimantCNIC = dto.ClaimantCNIC;
            entity.Address = dto.Address;
            entity.IsActiveClaimant = dto.IsActiveClaimant;
            entity.AccountNumber = dto.AccountNumber;
            entity.MonthlyPension = dto.MonthlyPension;
            entity.CMA = dto.CMA;
            entity.OrderelyAllowence = dto.OrderelyAllowence;
            entity.Total = dto.Total;
            entity.Remarks = dto.Remarks;
            entity.RelationId = dto.RelationId;
            entity.BPS = dto.BPS;
            entity.LastBasicPay = dto.LastBasicPay;
            entity.BranchId = dto.BranchId;
            entity.DateOfAppointment = dto.DateOfAppointment;
            entity.AccountTitle = dto.AccountTitle;
            entity.IsServiceActive = dto.IsServiceActive;
            entity.Commutation = dto.Commutation;
            entity.RetiringOffice = dto.RetiringOffice;
            entity.MonthlyRecovery = dto.MonthlyRecovery;
            entity.IBAN = dto.IBAN;
            return entity;
        }

        private static bool ValidateRow(string[] values, out int pduId, out int ppoNumber, out string name,
            out string designation, out int bps, out string claimant,
            out string claimantCnic, out string companyName, out string bankName,
            out string branchCode, out string accountNumber, out decimal mp,
            out decimal cma, out decimal orderly, out decimal total)
        {
            pduId = ppoNumber = bps = 0;
            name = designation = claimant = claimantCnic = companyName = bankName = branchCode = accountNumber = string.Empty;
            mp = cma = orderly = total = 0;

            if (values.Length < 15 ||
                !int.TryParse(values[0], out pduId) ||
                !int.TryParse(values[1], out ppoNumber) ||
                !int.TryParse(values[4], out bps) ||
                !decimal.TryParse(values[11], out mp) ||
                !decimal.TryParse(values[12], out cma) ||
                !decimal.TryParse(values[13], out orderly) ||
                !decimal.TryParse(values[14], out total))
            {
                return false;
            }

            name = values[2];
            designation = values[3];
            claimant = values[5];
            claimantCnic = values[6];
            companyName = values[7];
            bankName = values[8];
            branchCode = values[9];
            accountNumber = values[10];

            return true;
        }

        #endregion
    }
}