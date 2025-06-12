using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.DTOs;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;
using System.Data;
using System.Text;
using System.Text.Json;
using PensionSystem.Entities.DTOs;
using WebAPI.Helpers;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser")]
    public class PensionersController(ApplicationDbContext context,
        IRelation relation,
        ICompany company,
        IPensioner pensioner,
        IWebHostEnvironment webHostEnvironment,
        IBank bank, IBranch branch, IPDU pDU,
        SessionHelper sessionHelper,
 IHttpClientFactory httpClientFactory, IConfiguration configuration) : Controller
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

        // GET: Pensioners
        public IActionResult Index()
        { return View(); }

        [HttpGet]
        public IActionResult Files()
        { return PartialView("_Files"); }

        [HttpGet]
        public async Task<IActionResult> Increase()
        { return View(await _pensioner.GetActive(_sessionHelper.GetUserPDUId())); }

        [HttpGet]
        public IActionResult BulkAddition()
        { return View(); }

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
                // Use parsedPduId as needed
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
        public async Task<IActionResult> GetSMSList()
        { return PartialView("_SMSService", await _pensioner.GetSMSList(_sessionHelper.GetUserPDUId())); }

        public async Task<IActionResult> CreateUpdate(int? id)
        {
            PensionerDTO pM = new();
            if (id == null || id == 0)
            {
                ViewData["CompanyId"] = new SelectList(await _company.GetOptions(), "Value", "Text");
                ViewData["RelationId"] = new SelectList(await _relation.GetOptions(), "Value", "Text");
                ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text");
                return PartialView("_CreateUpdate", pM);
            }
            else
            {
                var p = await _context.Pensioner
                    .Include(x => x.Branch)
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();
                if (p != null)
                {
                    pM.Id = p.Id;
                    pM.PageNumber = p.PageNumber;
                    pM.Name = p.Name;
                    pM.Claimant = p.Claimant;
                    pM.FatherName = p.FatherName;
                    pM.Gender = p.Gender;
                    pM.Spouse = p.Spouse;
                    pM.Designation = p.Designation;
                    pM.PPONumber = p.PPONumber;
                    pM.PPOSystem = p.PPOSystem;
                    pM.SanctionNumber = p.SanctionNumber;
                    pM.SanctionDate = p.SanctionDate;
                    pM.DOB = p.DOB;
                    pM.DateOfRetirement = p.DateOfRetirement;
                    pM.CompanyId = p.CompanyId;
                    pM.Mobile = p.Mobile;
                    pM.CNIC = p.CNIC;
                    pM.ClaimantCNIC = p.ClaimantCNIC;
                    pM.Address = p.Address;
                    pM.IsActiveClaimant = p.IsActiveClaimant;
                    pM.AccountNumber = p.AccountNumber;
                    pM.MonthlyPension = p.MonthlyPension;
                    pM.CMA = p.CMA;
                    pM.OrderelyAllowence = p.OrderelyAllowence;
                    pM.Total = p.Total;
                    pM.Remarks = p.Remarks;
                    pM.RelationId = p.RelationId;
                    pM.BPS = p.BPS;
                    pM.BranchId = p.BranchId;
                    pM.DateOfAppointment = p.DateOfAppointment;
                    pM.LastBasicPay = p.LastBasicPay;
                    pM.AccountTitle = p.AccountTitle;
                    pM.IsServiceActive = p.IsServiceActive;
                    pM.RetiringOffice = p.RetiringOffice;
                    pM.Commutation = p.Commutation;
                    pM.MonthlyRecovery = p.MonthlyRecovery;
                    pM.IBAN = p.IBAN;
                    ViewData["CompanyId"] = new SelectList(await _company.GetOptions(), "Value", "Text", p.CompanyId);
                    ViewData["RelationId"] = new SelectList(await _relation.GetOptions(), "Value", "Text", p.RelationId);
                    ViewData["BankId"] = new SelectList(await _bank.GetOptions(), "Value", "Text", p.Branch.BankId);
                    ViewData["BranchId"] = new SelectList(await _branch.GetOptions(), "Value", "Text", p.BranchId);
                }
                return PartialView("_CreateUpdate", pM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update([Bind("Id","CompanyId", "PageNumber", "PPONumber", "PPOSystem", "Name", "Designation", "BPS",
            "FatherName","Mobile","CNIC","DOB","DateOfRetirement","RelationId","SanctionNumber","SanctionDate","Gender","Claimant","ClaimantCNIC","Spouse",
            "Address","AccountNumber","MonthlyPension","CMA","OrderelyAllowence","Total",
            "MonthlyRecovery","Remarks","IsActiveClaimant","BranchId","LastBasicPay",
            "DateOfAppointment","AccountTitle","IsServiceActive","Commutation","RetiringOffice","IBAN")] PensionerDTO pM)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                if (pM.Id == 0)
                {
                    Pensioner p = new()
                    {
                        PageNumber = pM.PageNumber,
                        Name = pM.Name,
                        Claimant = pM.Claimant,
                        FatherName = pM.FatherName,
                        Gender = pM.Gender,
                        Spouse = pM.Spouse,
                        Designation = pM.Designation,
                        PPONumber = pM.PPONumber,
                        PPOSystem = pM.PPOSystem,
                        SanctionNumber = pM.SanctionNumber,
                        SanctionDate = pM.SanctionDate,
                        DOB = pM.DOB,
                        DateOfRetirement = pM.DateOfRetirement,
                        CompanyId = pM.CompanyId,
                        Mobile = pM.Mobile,
                        CNIC = pM.CNIC,
                        ClaimantCNIC = pM.ClaimantCNIC,
                        Address = pM.Address,
                        IsActiveClaimant = pM.IsActiveClaimant,
                        AccountNumber = pM.AccountNumber,
                        MonthlyPension = pM.MonthlyPension,
                        CMA = pM.CMA,
                        OrderelyAllowence = pM.OrderelyAllowence,
                        Total = pM.Total,
                        Remarks = pM.Remarks,
                        RelationId = pM.RelationId,
                        BPS = pM.BPS,
                        LastBasicPay = pM.LastBasicPay,
                        BranchId = pM.BranchId,
                        DateOfAppointment = pM.DateOfAppointment,
                        AccountTitle = pM.AccountTitle,
                        IsServiceActive = pM.IsServiceActive,
                        PDUId = _sessionHelper.GetUserPDUId(),
                        Commutation = pM.Commutation,
                        RetiringOffice = pM.RetiringOffice,
                        MonthlyRecovery = pM.MonthlyRecovery,
                        IBAN = pM.IBAN
                    };
                    await _context.AddAsync(p);
                }
                else
                {
                    var p = await _context.Pensioner.Where(x => x.Id == pM.Id).SingleOrDefaultAsync();
                    if (p == null)
                    {
                        helper.RCode = 0;
                        helper.RText = "Unable to find in DB";
                        return Json(helper);
                    }
                    p.PageNumber = pM.PageNumber;
                    p.Name = pM.Name;
                    p.Claimant = pM.Claimant;
                    p.FatherName = pM.FatherName;
                    p.Gender = pM.Gender;
                    p.Spouse = pM.Spouse;
                    p.Designation = pM.Designation;
                    p.PPONumber = pM.PPONumber;
                    p.PPOSystem = pM.PPOSystem;
                    p.SanctionNumber = pM.SanctionNumber;
                    p.SanctionDate = pM.SanctionDate;
                    p.DOB = pM.DOB;
                    p.DateOfRetirement = pM.DateOfRetirement;
                    p.CompanyId = pM.CompanyId;
                    p.Mobile = pM.Mobile;
                    p.CNIC = pM.CNIC;
                    p.ClaimantCNIC = pM.ClaimantCNIC;
                    p.Address = pM.Address;
                    p.IsActiveClaimant = pM.IsActiveClaimant;
                    p.AccountNumber = pM.AccountNumber;
                    p.MonthlyPension = pM.MonthlyPension;
                    p.CMA = pM.CMA;
                    p.OrderelyAllowence = pM.OrderelyAllowence;
                    p.Total = pM.Total;
                    p.Remarks = pM.Remarks;
                    p.RelationId = pM.RelationId;
                    p.BPS = pM.BPS;
                    p.LastBasicPay = pM.LastBasicPay;
                    p.BranchId = pM.BranchId;
                    p.DateOfAppointment = pM.DateOfAppointment;
                    p.AccountTitle = pM.AccountTitle;
                    p.IsServiceActive = pM.IsServiceActive;
                    p.Commutation = pM.Commutation;
                    p.RetiringOffice = pM.RetiringOffice;
                    p.MonthlyRecovery = pM.MonthlyRecovery;
                    p.IBAN = pM.IBAN;
                    _context.Update(p);
                }
                try
                {
                    await _context.SaveChangesAsync();
                    helper.RCode = 1;
                }
                catch (DbUpdateConcurrencyException exc)
                {
                    helper.RText = ExceptionHelper.GetDetail(exc);
                }
            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var item in allErrors)
                {
                    helper.RText += item.ErrorMessage;
                }
            }
            return Json(helper);
        }

        // GET: Pensioners/Delete/5
        [Authorize(Roles = "PDUUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pensioner = await _context.Pensioner
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pensioner == null)
            {
                return NotFound();
            }

            return View(pensioner);
        }

        // POST: Pensioners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pensioner = await _context.Pensioner.FindAsync(id);
            if (pensioner != null)
            {
                _context.Pensioner.Remove(pensioner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }

        private bool PensionerExists(int id)
        {
            return _context.Pensioner.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> GetRestored()
        {
            return PartialView("_Restored", await _pensioner.GetRestored(_sessionHelper.GetUserPDUId()));
        }

        [HttpGet]
        [HttpPost]
        public async Task<bool> RestoreNow(int PensionerId)
        {
            // Retrieve the user-specific global variable from the session

            return await _pensioner.RestoreNow(PensionerId);
        }

        [HttpPost]
        public async Task<JsonResult?> GetMaxPageNumber(int CompanyId)
        {
            int MaxNumber = await _pensioner.GetMaxPageNumber(CompanyId);
            MaxNumber++;
            return Json(new { MaxNumber });
        }

        [HttpPost]
        public async Task<JsonResult> UploadFileOfRatesRevision(IFormFile file)
        {
            JsonResponseHelper result = new();
            try
            {
                if (file == null || file.Length == 0)
                {
                    result.RText = "File Not Selected";
                }

                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".csv")
                    result.RText = "File Not Selected";

                var rootFolder = Path.Combine(_hostingEnvironment.WebRootPath, "files", "rateRevisionFolder");
                Directory.CreateDirectory(rootFolder); // Create directory if it doesn't exist

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(rootFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                if (file.Length <= 0)
                    result.RText = "File has no data";

                var pContext = _context.Pensioner;
                if (pContext != null)
                {
                    // checking for each row
                    using (var reader = new StreamReader(filePath, Encoding.Default, false))
                    {
                        int Id = 0;
                        decimal MP = 0;
                        decimal CMA = 0;
                        decimal Orderly = 0;
                        decimal Total = 0;
                        int rowNumber = 1;
                        string currentLine;
                        while ((currentLine = reader.ReadLine()) != null)
                        {
                            // we will skip header row
                            if (rowNumber != 1)
                            {
                                var values = currentLine.Split(',');
                                Id = Convert.ToInt32(values[0]);
                                MP = Convert.ToDecimal(values[1]);
                                CMA = Convert.ToDecimal(values[2]);
                                Orderly = Convert.ToDecimal(values[3]);
                                Total = Convert.ToDecimal(values[4]);
                                // modifing data in DB
                                var currentP = await pContext.FindAsync(Id);
                                if (currentP != null)
                                {
                                    currentP.MonthlyPension = MP;
                                    currentP.CMA = CMA;
                                    currentP.OrderelyAllowence = Orderly;
                                    currentP.Total = Total;
                                    pContext.Update(currentP);
                                }
                                else
                                {
                                    result.RText = Id + " this Id not found ind DB in row NUmber" + rowNumber;
                                    return Json(result);
                                }
                            }

                            rowNumber++;
                        }
                    }
                    await _context.SaveChangesAsync();
                    result.RText = "Ok";
                    result.RCode = 1;
                }
                else
                { result.RText = "Error in DB"; }
            }
            catch (Exception exc)
            {
                result.RText = exc.Message.ToString();
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<bool> UpdateSMSServiceStatus(int PensionerId, bool Status)
        {
            return await _pensioner.UpdateSMSServiceStatus(PensionerId, Status);
        }

        [HttpPost]
        public async Task<JsonResult> UploadFileOfBulkAdditionOfPensioner(IFormFile file)
        {
            JsonResponseHelper result = new();
            try
            {
                if (file == null || file.Length == 0)
                {
                    result.RText = "File Not Selected";
                }

                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".csv")
                    result.RText = "File Not Selected";

                var rootFolder = Path.Combine(_hostingEnvironment.WebRootPath, "files", "BulkAdditionOfPensionerFolder");
                Directory.CreateDirectory(rootFolder); // Create directory if it doesn't exist

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(rootFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                if (file.Length <= 0)
                {
                    result.RText = "File has no data";
                    return Json(result);
                }

                // checking for each row
                using var reader = new StreamReader(filePath, Encoding.Default, false);
                int rowNumber = 1;
                string currentLine;
                var pensionUploadVM = new List<PensionerUploadVM>();
                while ((currentLine = reader.ReadLine()) != null)
                {
                    // we will skip header row
                    if (rowNumber != 1)
                    {
                        var values = currentLine.Split(',');
                        // validating data
                        if (ValidateRow(values, out int PUDId, out int PPONumber, out string Name,
                                        out string Designation, out int BPS, out string Claimant,
                                        out string ClaimantCNIC, out string CompanyShortName, out string BankShortName,
                                        out string BranchCode, out string AccountNumber, out decimal MonthlyPension,
                                        out decimal CMA, out decimal Orderly, out decimal Total))
                        {
                            // Process the validated data (e.g., modify the database)
                            if (Name.Length > 19)
                            {
                                result.RText = $"PPO # {PPONumber}Name must not exceed 19 Characters";
                                return Json(result);
                            }
                            if (Designation.Length > 50)
                            {
                                result.RText = $"PPO # {PPONumber} Designation " +
                                    $"must not exceed 50 Characters";
                                return Json(result);
                            }
                            if (AccountNumber.Length > 19)
                            {
                                result.RText = $"PPO # {PPONumber} Account Number " +
                                    $"must not exceed 19 Characters";
                                return Json(result);
                            }
                            if (Claimant.Length > 50)
                            {
                                result.RText = $"PPO # {PPONumber} Claimant " +
                                    $"must not exceed 50 Characters";
                                return Json(result);
                            }
                            if (ClaimantCNIC.Length > 15)
                            {
                                result.RText = $"PPO # {PPONumber} ClaimantCNIC " +
                                    $"must not exceed 15 Characters";
                                return Json(result);
                            }
                            pensionUploadVM.Add(new PensionerUploadVM()
                            {
                                PDUId = PUDId,
                                PPONumber = PPONumber,
                                Name = Name,
                                Designation = Designation,
                                BPS = BPS,
                                CMA = CMA,
                                AccountNumber = AccountNumber,
                                BankShortName = BankShortName,
                                BranchCode = BranchCode,
                                Claimant = Claimant,
                                ClaimantCNIC = ClaimantCNIC,
                                CompanyShortName = CompanyShortName,
                                MonthlyPension = MonthlyPension,
                                Orderly = Orderly,
                                Total = Total
                            });
                        }
                        else
                        {
                            result.RText = $"Invalid data in PPO Number {PPONumber}";
                            Json(result);
                        }
                        // modifing data in DB
                    }
                    rowNumber++;
                }
                // it means data is correct now we will Upload it to DB
                var (IsSaved, Message) = await _pensioner.AddToDBFromFile(pensionUploadVM);
                result.RText = Message;
                result.RCode = IsSaved ? 1 : 0;
                return Json(result);
            }
            catch (Exception exc)
            {
                result.RText = exc.Message.ToString();
            }
            return Json(result);
        }

        private static bool ValidateRow(string[] values, out int PUDId, out int PPONumber, out string Name,
                           out string Designation, out int BPS, out string Claimant,
                           out string ClaimantCNIC, out string CompanyName, out string BankName,
                           out string BranchCode, out string AccountNumber, out decimal MP,
                           out decimal CMA, out decimal Orderly, out decimal Total)
        {
            // Data type validation for each field
            if (!int.TryParse(values[0], out PUDId) ||
                !int.TryParse(values[1], out PPONumber) ||
                !int.TryParse(values[4], out BPS) ||
                !decimal.TryParse(values[11], out MP) ||
                !decimal.TryParse(values[12], out CMA) ||
                !decimal.TryParse(values[13], out Orderly) ||
                !decimal.TryParse(values[14], out Total))
            {
                PPONumber = 0;
                Name = "";
                Designation = "";
                BPS = 0;
                Claimant = "";
                ClaimantCNIC = "";
                CompanyName = "";
                BankName = "";
                BranchCode = "";
                AccountNumber = "";
                MP = 0;
                CMA = 0;
                Orderly = 0;
                Total = 0;
                return false;
            }

            // Other validation checks (string lengths, patterns, etc.) for string fields
            // ...

            Name = values[2];
            Designation = values[3];
            Claimant = values[5];
            ClaimantCNIC = values[6];
            CompanyName = values[7];
            BankName = values[8];
            BranchCode = values[9];
            AccountNumber = values[10];

            // Perform additional validation checks for string fields
            // ...

            return true;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(FileUploadDTO fileUploadDTO)
        {
            var client = _httpClientFactory.CreateClient();
            // Access the BaseUrl from appsettings.json
            string baseUrl = _configuration["BaseUrl"];
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("" + baseUrl + "/api/Pensioner/Upload"),
                Content = new StringContent(JsonSerializer.Serialize(fileUploadDTO), Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadFromJsonAsync<FileUploadDTO>();
                return Ok(r);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Log the responseContent for debugging
                return BadRequest("Error: " + responseContent);
            }
        }

    }
}