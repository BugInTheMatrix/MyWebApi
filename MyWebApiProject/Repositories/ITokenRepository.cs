using Microsoft.AspNetCore.Identity;

namespace MyWebApiProject.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser userName,List<string> Roles);
    }
}
