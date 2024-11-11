using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using WebAPI.Interfaces;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WWFSanctionController(IMapper mapper, IWWFSanction wWFSanction) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IWWFSanction _Ientity = wWFSanction;

        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _Ientity.GetById(Id);
            if (r == null)
                return NotFound();

            var record = _mapper.Map<WWFSanctionDTO>(r);
            return Ok(record);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWWFSanctionDTO entity)
        {
            var r = _mapper.Map<WWFSanction>(entity);
            var (IsSaved, Message) = await _Ientity.Insert(r);
            return IsSaved ? CreatedAtAction(nameof(GetById), new { id = r.Id }, entity) : BadRequest(Message);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WWFSanctionDTO entity)
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
        [Route("GetByPDUId({id:int})")]
        public async Task<IActionResult> GetByPDUId(int id)
        {
            var r = await _Ientity.GetAll(id);
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
