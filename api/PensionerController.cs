﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.DTOs;
using PensionSystem.Entities.DTOs;
using PensionSystem.Interfaces;

namespace PensionSystem.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PensionerController(IPensioner pensioner, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IMapper mapper) : ControllerBase
    {
        private readonly IPensioner _pensioner = pensioner;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor = httpContextAccessor;
        private readonly IMapper _mapper = mapper;

        //[HttpPost]
        //[Route("Upload")]
        //public async Task<IActionResult> Upload([FromForm] FileUploadDTO model)
        //{
        //    if (model.File == null || model.File.Length == 0)
        //    {
        //        return BadRequest("No file uploaded.");
        //    }
        //    if (model.File.Length > 5000000)
        //    {
        //        ModelState.AddModelError("file", "Maximum file size 5MP");
        //        return BadRequest(ModelState);
        //    }
        //    var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "/" + model.Type + "/", model.File.FileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await model.File.CopyToAsync(stream);
        //    }

        //    return Ok("Image uploaded successfully.");
        //}

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn,
           [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 1000)
        {
            var list = await _pensioner.GetAll(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(list);
        }
        // Get Pensioner By Id
        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _pensioner.GetById(Id);
            if (r == null) return NotFound();
            return Ok(r);

        }
        [HttpGet]
        [Route("GetOptions")]
        public async Task<IActionResult> GetOptions()
        {
            var l = await _pensioner.GetAll(1);
            if (l == null) return NotFound();
            var r = _mapper.Map<List<PensionerOptionDTO>>(l);
            return Ok(r);

        }
        [HttpPut]
        [Route("UpdateAccountNumber")]
        public async Task<IActionResult> UpdateAccountNumber(UpdateBranchDTO updateBranchDTO)
        {
            return await _pensioner.UpdateAccountNumber(updateBranchDTO) == true ? Ok() : BadRequest();
        }
    }
}