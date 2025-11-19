using MyWebApiProject.Models.Domain;

namespace MyWebApiProject.Repositories
{
    public interface IWalkRepository
    {
        Task<Walk> Create(Walk walk);
        Task<List<Walk>> GetAllAsync(string? filterOn=null,string? filterQuery=null);
        Task<Walk?> GetByIdAsync(Guid id);
        Task<Walk?> UpdateAsync(Guid id, Walk walk);
        Task<Walk?> Delete(Guid id);
    }
}
