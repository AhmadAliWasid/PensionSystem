using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pension.Entities.Helpers;
using PensionSystem.Data;
using PensionSystem.Entities.Models;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.ViewModels;
using System.Globalization;
using System.Text;

namespace PensionSystem.Controllers
{
    [Authorize(Roles = "PDUUser,Administrator")]
    public class ArrearsPaymentsController(ApplicationDbContext context, IArrearsPayment arrearsPayment,
        IArrearsDemand arrearsDemand, IPensioner pensioner, IWebHostEnvironment webHostEnvironment, SessionHelper sessionHelper) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IArrearsPayment _AP = arrearsPayment;
        private readonly IArrearsDemand _AD = arrearsDemand;
        private readonly IPensioner _IP = pensioner;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly SessionHelper _sessionHelper = sessionHelper;

        // GET: ArrearsPayments
        public async Task<IActionResult> Index()
        {
            var cContext = _context.ArrearsPayments;
            if (cContext != null)
            {
                ViewData["ArrearsDemandId"] = new SelectList(await _AD.GetOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
                var ArrearsPayments = await cContext
                    .Include(x => x.Pensioner)
                    .Include(y => y.ArrearsDemand)
                    .OrderBy(y => y.Pensioner.PPOSystem)
                    .ToListAsync();
                ArrearsPaymentViewModel model = new()
                {
                    ArrearsPayments = ArrearsPayments
                };
                return View(model);
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> Crud(int id)
        {
            if (id == 0)
            {
                ViewData["ArrearsDemandId"] = new SelectList(await _AD.GetUnpaidOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
                ViewData["PensionerId"] = new SelectList(await _IP.GetOptions(_sessionHelper.GetUserPDUId(), true), "Value", "Text");
                var ap = new ArrearsPayment
                {
                    FromMonth = DateTime.Now,
                    ToMonth = DateTime.Now,
                    Month = DateTime.Now
                };
                return View("_Crud", ap);
            }
            else
            {
                var r = await _AP.Get(id);
                if (r != null)
                {
                    ViewData["ArrearsDemandId"] = new SelectList(await _AD.GetUnpaidOptions(_sessionHelper.GetUserPDUId()), "Value", "Text", r.ArrearsDemandId);
                    ViewData["PensionerId"] = new SelectList(await _IP.GetOptions(_sessionHelper.GetUserPDUId(), true), "Value", "Text", r.PensionerId);
                }
                return View("_Crud", r);
            }
        }
        public async Task<JsonResult> Save(ArrearsPayment vM)
        {
            JsonResponseHelper helper = new();
            if (ModelState.IsValid)
            {
                var demand = await _AD.GetById(vM.ArrearsDemandId);
                if (demand != null && demand.IsPaid) {
                    helper.RText = "Demand is locked"!;
                    helper.RCode = 0;
                    return Json(helper);

                }
                var (IsSaved, Message) = vM.Id == 0 ? await _AP.Insert(vM) : await _AP.Update(vM);
                helper.RCode = IsSaved ? 1 : 0;
                helper.RText = Message;
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
        // GET: ArrearsPayments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ArrearsDemandId"] = new SelectList(await _AD.GetUnpaidOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            ViewData["PensionerId"] = new SelectList(await _IP.GetOptions(_sessionHelper.GetUserPDUId(), true), "Value", "Text");
            return View();
        }

        // GET: ArrearsPayments/BulkUpload
        public async Task<IActionResult> BulkUpload()
        {
            var vm = new UploadPaymentVM()
            {
                Pensioners = await _IP.GetActive(_sessionHelper.GetUserPDUId())
            };
            ViewData["ArrearsDemandId"] = new SelectList(await _AD.GetUnpaidOptions(_sessionHelper.GetUserPDUId()), "Value", "Text");
            return View(vm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arrearsPayment = await _context.ArrearsPayments
                .Include(a => a.Pensioner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrearsPayment == null)
            {
                return NotFound();
            }

            return View(arrearsPayment);
        }

        // POST: ArrearsPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _AP.GetById(id);
            if (r == null)
                return NotFound();
            else
                await _AP.Delete(r);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> GetInfo(int id)
        {
            return Json(await _IP.Get(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetDemandList(int demandId)
        {
            DemandListViewModel viewModel = new()
            {
                Month = _AD.GetMonth(demandId),
                ArrearsPayments = await _AP.GetByDemand(demandId)
            };
            var bankList = viewModel.ArrearsPayments
                .GroupBy(x => x.Pensioner.Branch.Bank)
                .ToList();
            var bankVm = new List<BankDemandVM>();
            foreach (var bank in bankList)
            {
                bankVm.Add(new BankDemandVM
                {
                    BankName = bank.Key.ShortName,
                    Amount = bank.Sum(y => y.Total),
                    Count = bank.Count()
                });
            }
            viewModel.bankDemandVMs = bankVm;
            viewModel.Session = new SessionViewModel()
            {
                AMStamp = _sessionHelper.GetAMStamp(),
                DMStamp = _sessionHelper.GetDMStamp(),
                BaseStamp = _sessionHelper.GetBaseStamp()
            };
            return PartialView("_DemandListPensioner", viewModel);
        }

        [HttpGet]
        public async Task<JsonResult> ClearArrearDemand(int demandId)
        {
            // clearing demand items
            var result = await _AP.ClearDemand(demandId);
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> IsValid(int PensionerId, DateTime StartingMonth, DateTime EndingMonth)
        {
            var (Valid, Msg) = await _AP.IsValid(PensionerId, StartingMonth, EndingMonth);
            bool isValid = Valid;
            string MsgResponse = Msg;
            return Json(new { isValid, MsgResponse });
        }

        [HttpPost]
        public async Task<JsonResult> UploadArrearPayments(IFormFile file, int ArrearsDemandId)
        {
            JsonResponseHelper result = new();
            try
            {
                if (file == null || file.Length == 0)
                {
                    result.RText = "File Not Selected";
                    return Json(result);
                }
                if (ArrearsDemandId == 0)
                {
                    result.RText = "No Demand is selected";
                    return Json(result);
                }
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".csv")
                {
                    result.RText = "File Not Selected";
                    return Json(result);
                }

                // uploading file
                var rootFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ArrearPayments");
                Directory.CreateDirectory(rootFolder); // Create directory if it doesn't exist
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(rootFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // now will start uploading process
                var resultUpload = await InsertArrears(filePath, ArrearsDemandId);
                return Json(resultUpload);
            }
            catch (Exception exc)
            {
                result.RText = exc.Message.ToString();
            }
            return Json(result);
        }

        private async Task<JsonResult> InsertArrears(string filePath, int ArrearsDemandId)
        {
            JsonResponseHelper result = new();

            try
            {
                var demand = await _AD.Get(ArrearsDemandId);
                if (demand == null)
                {
                    result.RText = "Demand not found";
                    return Json(result);
                }

                using (var reader = new StreamReader(filePath, Encoding.Default, false))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var arrearPayments = csv.GetRecords<UploadPaymentInsertModel>().ToList();

                    // Process arrearPayments here
                    var aContext = _context.ArrearsPayments;
                    if (aContext == null)
                    {
                        result.RText = "Unable to find arrear table";
                        return Json(result);
                    }
                    foreach (var r in arrearPayments)
                    {
                        await aContext.AddAsync(new ArrearsPayment()
                        {
                            ArrearsDemandId = ArrearsDemandId,
                            Description = r.Description,
                            MP = r.MP,
                            CMA = r.CMA,
                            Orderly = r.Orderly,
                            Total = r.Total,
                            FromMonth = r.From,
                            ToMonth = r.To,
                            NumberOfMonths = r.Months,
                            Month = demand.Date,
                            Deduction = r.Deduction,
                            NetPension = r.Net,
                            PensionerId = r.PensionerId,
                            CreatedDate = DateTime.Now
                        });
                    }
                    await _context.SaveChangesAsync();
                    result.RText = "Arrear Payment Added Successfully";
                }
            }
            catch (Exception exc)
            {
                result.RCode = 0;
                result.RText = ExceptionHelper.GetDetail(exc);
            }
            return Json(result);
        }
    }
}