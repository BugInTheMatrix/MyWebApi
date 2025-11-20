using MyWebApiProject.Models.Domain;

namespace MyWebApiProject.Repositories
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
