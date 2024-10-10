using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Entities.DTOs;
using PensionSystem.Interfaces;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankPaymentController(IMapper mapper, IHBLPayments hBLPayments) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IHBLPayments _entity = hBLPayments;
        [HttpGet]
        [Route("{PensionerId:int}")]
        public async Task<IActionResult> GetByPensionerId([FromRoute] int PensionerId)
        {
            var e = await _entity.GetByPensionerId(PensionerId);
            if (e == null)
                return NotFound();
            else
                return Ok(_mapper.Map<HBLPaymentDTO>(e));
        }
    }
}
