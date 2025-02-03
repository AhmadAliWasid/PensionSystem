using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Entities.DTOs;
using PensionSystem.Interfaces;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommutationsController(ICommutation commutation, IMapper mapper) : ControllerBase
    {
        private readonly ICommutation _commutation = commutation;
        private readonly IMapper _mapper = mapper;
        [HttpGet]
        [Route("GetByPDUId({Id:int})")]
        public async Task<IActionResult> GetByPDUId([FromRoute] int Id)
        {
            var r = await _commutation.GetAll(Id);
            if (r == null)
                return NotFound();

            var record = _mapper.Map<List<CommutationDTO>>(r);
            return Ok(record);
        }

        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _commutation.GetById(Id);
            if (r == null)
                return NotFound();

            var record = _mapper.Map<CommutationDTO>(r);
            return Ok(record);
        }
    }
}
