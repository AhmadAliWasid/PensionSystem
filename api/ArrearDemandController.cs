using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using PensionSystem.Interfaces;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArrearDemandController(IMapper mapper, IArrearsDemand arrearsDemand) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IArrearsDemand _arrearsDemand = arrearsDemand;
        [HttpGet]
        [Route("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            var r = await _arrearsDemand.Get(Id);
            if (r == null)
                return NotFound();

            var record = _mapper.Map<UpdateArreardDemandDTO>(r);
            return Ok(record);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArreardDemandDTO entity)
        {
            var r = _mapper.Map<ArrearsDemand>(entity);
            var (IsSaved, Message) = await _arrearsDemand.Insert(r);
            return IsSaved ? CreatedAtAction(nameof(GetById), new { id = r.Id }, entity) : BadRequest(Message);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateArreardDemandDTO entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("ID mismatch");
            }
            var existingEntity = await _arrearsDemand.Get(id);
            if (existingEntity == null)
            {
                return NotFound();
            }
            var updatedEntity = _mapper.Map(entity, existingEntity);
            var (IsUpdated, Message) = await _arrearsDemand.Update(updatedEntity);
            return IsUpdated ? Ok() : BadRequest(Message);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var r = await _arrearsDemand.GetAll(null);
            return Ok(r);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingEntity = await _arrearsDemand.Get(id);
            if (existingEntity == null)
            {
                return NotFound();
            }
            var r = await _arrearsDemand.Delete(existingEntity);
            return r ? NoContent() : BadRequest(r);
        }


    }
}
