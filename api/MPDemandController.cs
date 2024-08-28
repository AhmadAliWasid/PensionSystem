using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Interfaces;
using PensionSystem.Entities.Models;
using PensionSystem.Entities.DTOs;

namespace PensionSystem.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MPDemandController(IMonthlyPensionDemand monthlyPensionDemandService, IMapper mapper) : ControllerBase
    {
        private readonly IMonthlyPensionDemand _mpDemand = monthlyPensionDemandService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _mpDemand.GetById(Id);
            if (r == null)
                return NotFound();

            var record = _mapper.Map<UpdateMPDemandDTO>(r);
            return Ok(record);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMPDemandDTO createMPDemand)
        {
            var r = _mapper.Map<MonthlyPensionDemand>(createMPDemand);
            var (IsSaved, Message) = await _mpDemand.Insert(r);
            return IsSaved ? CreatedAtAction(nameof(GetById), new { id = r.Id }, createMPDemand) : BadRequest(Message);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateMPDemandDTO updateMPDemandDTO)
        {
            var r = _mapper.Map<MonthlyPensionDemand>(updateMPDemandDTO);
            var result = await _mpDemand.Update(r);
            return result.IsSaved ? Ok(result) : BadRequest(result);
        }
    }
}
