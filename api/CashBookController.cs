using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.Services;
using PensionSystem.Entities.DTOs;

namespace PensionSystem.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashBookController(ICashBook cashBook, IMapper mapper) : ControllerBase
    {
        private readonly ICashBook _cashBook = cashBook;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _cashBook.GetById(Id);
            if (r == null)
                return NotFound();

            var record = _mapper.Map<UpdateCashBookDTO>(r);
            return Ok(record);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int PDUId, [FromQuery] string? filterOn,
                [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 1000)
        {
            var list = await _cashBook.GetAll(PDUId, filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(list);
        }
    }
}
