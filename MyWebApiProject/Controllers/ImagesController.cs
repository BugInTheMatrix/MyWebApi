using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApiProject.Models.Domain;
using MyWebApiProject.Models.DTO;
using MyWebApiProject.Repositories;

namespace MyWebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }
        [HttpPost]
        [Route("Uplaod")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto imageUploadRequestDto)
        {
            ValidateFileUplaod(imageUploadRequestDto);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageDOmainModel = new Image
            {
                ImageFile = imageUploadRequestDto.ImageFile,
                FileName = imageUploadRequestDto.FileName,
                FileDescription = imageUploadRequestDto.FileDescription,
                FileExtension = Path.GetExtension(imageUploadRequestDto.ImageFile.FileName),
                FileSizeInBytes = imageUploadRequestDto.ImageFile.Length,
            };
            await imageRepository.Upload(imageDOmainModel);
            return Ok(imageDOmainModel);
        }
        private void ValidateFileUplaod(ImageUploadRequestDto imageUploadRequestDto)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(imageUploadRequestDto.ImageFile.FileName)))
            {
                ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, and .png files are allowed.");

            }
            if(imageUploadRequestDto.ImageFile.Length > 5*1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "File size cannot exceed 5 MB.");
            }
        }
    }
}
