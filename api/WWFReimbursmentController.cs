using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using System.Security.Principal;
using WebAPI.Interfaces;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WWFReimbursmentController(IMapper mapper, IWWFReimbursment wWFReimbursment) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IWWFReimbursment _Ientity = wWFReimbursment;
        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _Ientity.GetById(Id);
            if (r == null)
                return NotFound();

            var record = _mapper.Map<WWFReimbursmentDTO>(r);
            return Ok(record);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWWFReimbursmentDTO entity)
        {
            var r = _mapper.Map<WGReimbursment>(entity);
            var (IsSaved, Message) = await _Ientity.Insert(r);
            return IsSaved ? CreatedAtAction(nameof(GetById), new { id = r.Id }, entity) : BadRequest(Message);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WWFReimbursmentDTO entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("ID mismatch");
            }
            var existingEntity = await _Ientity.GetById(id);
            if (existingEntity == null)
            {
                return NotFound();
            }
            var updatedEntity = _mapper.Map(entity, existingEntity);
            var (IsUpdated, Message) = await _Ientity.Update(updatedEntity);
            return IsUpdated ? Ok() : BadRequest(Message);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var r = await _Ientity.GetAll(1);
            return Ok(r);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingEntity = await _Ientity.GetById(id);
            if (existingEntity == null)
            {
                return NotFound();
            }
            var r = await _Ientity.Delete(existingEntity);
            return r ? NoContent() : BadRequest(r);
        }

    }
}
