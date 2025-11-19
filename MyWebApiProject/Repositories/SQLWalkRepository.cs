using Microsoft.EntityFrameworkCore;
using MyWebApiProject.Data;
using MyWebApiProject.Models.Domain;

namespace MyWebApiProject.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        public SQLWalkRepository(NZWalksDbContext nZWalksDbContext) 
        {
            this.nZWalksDbContext = nZWalksDbContext;

        }
        public async Task<Walk> Create(Walk walk)
        {
            await nZWalksDbContext.Walks.AddAsync(walk);
            await nZWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> Delete(Guid id)
        {
            var esistingwalk= await nZWalksDbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x=>x.Id == id);
            if(esistingwalk == null)
            {
                return null;
            }
            nZWalksDbContext.Walks.Remove(esistingwalk);
            await nZWalksDbContext.SaveChangesAsync();
            return esistingwalk;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null)
        {
            var walks= nZWalksDbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();
            if(!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if(filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
            }
            return await walks.ToListAsync();
            //return await nZWalksDbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            var walk = await nZWalksDbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
            if(walk == null)
            {
                return null;
            }
            return walk;
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var existingWalk = await nZWalksDbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
            if(existingWalk == null)
            {
                return null;
            }
            existingWalk.Name = walk.Name;
            existingWalk.Description = walk.Description;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.WalkImageUrl = walk.WalkImageUrl;
            existingWalk.DifficultyId = walk.DifficultyId;
            existingWalk.RegionId = walk.RegionId;
            await nZWalksDbContext.SaveChangesAsync();
            return existingWalk;

        }
    }
}
