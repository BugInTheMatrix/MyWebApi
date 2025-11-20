using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyWebApiProject.Data
{
    public class NzWalksAuthDbContext:IdentityDbContext
    {
        public NzWalksAuthDbContext(DbContextOptions<NzWalksAuthDbContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var readerId = "a7c0e887-08c3-4cbd-8185-1c6bc5da6ef8";
            var writerId = "6ee1c318-7904-4720-8320-cbca33d13e21";
            var Roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=readerId,
                    ConcurrencyStamp=readerId,
                    Name="Reader",
                    NormalizedName="READER".ToUpper()
                },
                new IdentityRole
                {
                    Id=writerId,
                    ConcurrencyStamp=writerId,
                    Name="Writer",
                    NormalizedName="Writer".ToUpper()
                }
            };
            builder.Entity<IdentityRole>().HasData(Roles);

        }
    }
}
