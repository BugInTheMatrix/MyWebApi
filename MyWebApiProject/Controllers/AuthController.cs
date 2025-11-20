using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyWebApiProject.Models.DTO;
using MyWebApiProject.Repositories;

namespace MyWebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly UserManager<IdentityUser> usermanager;
        private readonly ITokenRepository tokenRepository;
        public AuthController(UserManager<IdentityUser> userManager,ITokenRepository tokenRepository) 
        {
            this.usermanager = userManager;
            this.tokenRepository = tokenRepository;

        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName=registerRequestDto.UserName,
                Email=registerRequestDto.UserName
            };
            var identityUserResult= await usermanager.CreateAsync(identityUser,registerRequestDto.Password);
            if (identityUserResult.Succeeded) 
            {
                if(registerRequestDto.Roles!=null && registerRequestDto.Roles.Any())
                {
                    await usermanager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                    if (identityUserResult.Succeeded)
                    {
                        return Ok("User was Register pleasse Login");
                    }

                }
            }
            return BadRequest("Something Went Wrong");

        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await usermanager.FindByEmailAsync(loginRequestDto.Username);
            if(user != null)
            {
                var checkPasswordResult = await usermanager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPasswordResult) 
                {
                    var roles = await usermanager.GetRolesAsync(user);
                    if(roles != null && roles.Any())
                    {
                        var token = tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JwtToken = token
                        };
                        return Ok(response);
                    }
                }
            }
            return BadRequest("Usrname or Password incorrect");
        }
    }
}
