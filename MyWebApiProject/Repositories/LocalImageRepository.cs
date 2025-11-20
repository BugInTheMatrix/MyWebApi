using MyWebApiProject.Data;
using MyWebApiProject.Models.Domain;

namespace MyWebApiProject.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly NZWalksDbContext nZWalksDbContext;
        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,IHttpContextAccessor httpContextAccessor,NZWalksDbContext nZWalksDbContext) 
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public async Task<Image> Upload(Image image)
        {
            var loaclFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");
            using var stream = new FileStream(loaclFilePath,FileMode.Create);
            await image.ImageFile.CopyToAsync(stream);
            //https://localhost:5001/Images/image.png
            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}" +
                $"{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;
            await nZWalksDbContext.Images.AddAsync(image);
            await nZWalksDbContext.SaveChangesAsync();
            return image;
        }
    }
}
