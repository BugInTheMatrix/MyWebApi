using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApiProject.CustomValidationFilter;
using MyWebApiProject.Data;
using MyWebApiProject.Models.Domain;
using MyWebApiProject.Models.DTO;
using MyWebApiProject.Repositories;

namespace MyWebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository sQLRegionRepository;
        private readonly IMapper automapper;
        public RegionsController(IRegionRepository sQLRegionRepository,IMapper mapper)
        {
            this.sQLRegionRepository = sQLRegionRepository;
            this.automapper = mapper;
        }
        //GET
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll()
        {
            var regions = await sQLRegionRepository.GetAllAsync();
            //var regionDtos = new List<RegionDto>();
            //foreach(var region in regions)
            //{
            //    regionDtos.Add(new RegionDto
            //    {
            //        Id= region.Id,
            //        Code = region.Code,
            //        Name = region.Name,
            //        RegionImageUrl = region.RegionImageUrl

            //    });
            //}
            var regionDtos = automapper.Map<List<RegionDto>>(regions);
            return Ok(regionDtos);
        }
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            //var region = nZWalksDbContext.Regions.Find(id);
            var region =await sQLRegionRepository.GetByIdAsync(id);
           
            if (region == null)
            {
                return NotFound();
            }
            //var regionDto = new RegionDto
            //{
            //    Id = region.Id,
            //    Code = region.Code,
            //    Name = region.Name,
            //    RegionImageUrl = region.RegionImageUrl
            //};
            var regionDto = automapper.Map<RegionDto>(region);
            return Ok(regionDto);
        }

        //POST
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var regionDomian = new Region
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name = addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};
            var regionDomian= automapper.Map<Region>(addRegionRequestDto);
            await sQLRegionRepository.CreateAsync(regionDomian);
            //var regionDto=new RegionDto
            //{
            //    Id = regionDomian.Id,
            //    Code = regionDomian.Code,
            //    Name = regionDomian.Name,
            //    RegionImageUrl = regionDomian.RegionImageUrl
            //};
            var regionDto = automapper.Map<RegionDto>(regionDomian);
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //var regionDomain=new Region
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            //};
            //if (!ModelState.IsValid) 
            //{
            //    return BadRequest(ModelState);
            //}
            var regionDomain = automapper.Map<Region>(updateRegionRequestDto);
            var rehionDOmain = await sQLRegionRepository.UpdateAsync(id, regionDomain);
            if (rehionDOmain == null)
            {
                return NotFound();
            }
            
            //var regionDto = new RegionDto
            //{
            //    Id = rehionDOmain.Id,
            //    Code = rehionDOmain.Code,
            //    Name = rehionDOmain.Name,
            //    RegionImageUrl = rehionDOmain.RegionImageUrl
            //};
            var regionDto = automapper.Map<RegionDto>(rehionDOmain);
            return Ok(regionDto);
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles ="Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomain= await sQLRegionRepository.DeleteAsync(id);
            if(regionDomain == null)
            {
                return NotFound();
            }
            
            //var regionDto=new RegionDto
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
            var regionDto = automapper.Map<RegionDto>(regionDomain);
            return Ok(regionDto);
        }
    }
}
