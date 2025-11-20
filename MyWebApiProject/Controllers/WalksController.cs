using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApiProject.Models.Domain;
using MyWebApiProject.Models.DTO;
using MyWebApiProject.Repositories;

namespace MyWebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper automapper;
        private readonly IWalkRepository walkRepository;
        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.automapper = mapper;
            this.walkRepository = walkRepository;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalkDto addWalkDto)
        {
            var walkDomain = automapper.Map<Walk>(addWalkDto);
            await walkRepository.Create(walkDomain);

            return Ok(automapper.Map<WalkDto>(walkDomain));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending,
            [FromQuery] int pageNumber=1, [FromQuery] int pageSize=100)
        {
            var walks = await walkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending,pageNumber,pageSize);
            var walkDtos = automapper.Map<List<WalkDto>>(walks);
            return Ok(walkDtos);

        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await walkRepository.GetByIdAsync(id);
            if (walk == null)
            {
                return NotFound();
            }
            var walkDto = automapper.Map<WalkDto>(walk);
            return Ok(walkDto);
        }
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walkDomain = automapper.Map<Walk>(updateWalkRequestDto);
            var updatedWalk = await walkRepository.UpdateAsync(id, walkDomain);
            if (updatedWalk == null)
            {
                return NotFound();
            }
            var updatedWalkDto = automapper.Map<WalkDto>(updatedWalk);
            return Ok(updatedWalkDto);

        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedWalk = await walkRepository.Delete(id);
            if (deletedWalk == null)
            {
                return NotFound();
            }
            var deletedWalkDto = automapper.Map<WalkDto>(deletedWalk);
            return Ok(deletedWalkDto);
        }
    }
}