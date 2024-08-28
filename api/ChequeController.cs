using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.DTOs;
using PensionSystem.Interfaces;

namespace PensionSystem.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequeController : ControllerBase
    {
        private readonly ICheque _cheque;
        private readonly IMapper _mapper;
        public ChequeController(ICheque cheque, IMapper mapper)
        {
            _cheque = cheque;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn,
          [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1,
          [FromQuery] int pageSize = 1000)
        {
            var list = await _cheque.GetAll(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(list);
        }
        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _cheque.GetById(Id);
            if (r == null)
                return NotFound();

            var chequeDTO = _mapper.Map<ChequeDTO>(r);
            return Ok(chequeDTO);
        }
    }
}
